namespace ToDoLater.Containerized.Infrastructure.Docker
{
    using global::Docker.DotNet;
    using global::Docker.DotNet.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ToDoLater.Containerized.Infrastructure.Abstraction;
    using ToDoLater.Containerized.Infrastructure.Docker.Models;
    using ToDoLater.Containerized.Infrastructure.Models;

    public abstract class DockerContainerizedService : ContainerizedService
    {
        internal readonly IDockerClient dockerClient;

        protected readonly DockerServiceSettings settings;

        public DockerContainerizedService(IDockerClient dockerClient, DockerServiceSettings settings)
        {
            this.dockerClient = dockerClient;
            this.settings = settings;
        }

        protected string DockerId { get; private set; }

        protected string DomainName { get; private set; }

        protected string Identifier { get; private set; }

        public override async Task BuildAsync(CancellationToken cancellationToken)
        {
            if (!this.settings.RemoteImage)
            {
                string dockerfilePath = new FileInfo(this.settings.DockerfilePath).FullName;
                var solutionDirectory = GetSolutionDirectory(new FileInfo(this.settings.DockerfilePath).Directory);
              

                var command = $"build -t \"{this.settings.Image}\" -f {dockerfilePath} .";
                var regex = new Regex(@"\s(([1-9])|([0-9][0-9]+))\serror\(s\)", RegexOptions.Singleline);

                bool receivedErrordata = false;
                StringBuilder output = new StringBuilder();
                using (var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        WorkingDirectory = solutionDirectory.FullName,
                        FileName = "docker",
                        Arguments = command, 
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
#if NETSTANDARD2_0
                        UseShellExecute = false
#endif
                    }
                })
                {
                    process.ErrorDataReceived += (sender, data) =>
                    {
                        if (data.Data != null && regex.Match(data.Data).Success)
                        {
                            receivedErrordata = true;
                        }
                        output.AppendLine(data.Data);
                    };
                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    if (receivedErrordata || process.ExitCode == 1)
                    {
                        throw new Exception($"Unable to generate docker image. \n {output}");
                    }
                }
            }

            await base.BuildAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task InitializeAsync(string networkId, string identifier, string domainName, CancellationToken cancellationToken)
        {
            this.DomainName = domainName;
            this.Identifier = identifier;

            if (!this.settings.RemoteImage)
            {
                await BuildAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await PullImageAsync(cancellationToken).ConfigureAwait(false);
            }

            this.DockerId = await this.CreateContainerAsync(networkId, cancellationToken).ConfigureAwait(false);

            await this.dockerClient.Containers.StartContainerAsync(this.DockerId, new ContainerStartParameters(), cancellationToken).ConfigureAwait(false);

            while (true)
            {
                if (this.State == ServiceState.Ready || await this.IsReadyAsync(cancellationToken).ConfigureAwait(false))
                {
                    this.State = ServiceState.Ready;
                    if (await this.PrepareAsync(cancellationToken).ConfigureAwait(false))
                    {
                        this.State = ServiceState.Prepared;
                        return;
                    }
                }

                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (this.DockerId != null)
            {
                using (var tokeSource = new CancellationTokenSource(20000))
                {
                    await this.dockerClient.Containers.StopContainerAsync(this.DockerId, new ContainerStopParameters(), tokeSource.Token).ConfigureAwait(false);
                    await this.dockerClient.Containers.RemoveContainerAsync(this.DockerId, new ContainerRemoveParameters { Force = true, RemoveVolumes = true }, tokeSource.Token).ConfigureAwait(false);
                }
            }

            GC.SuppressFinalize(this);
        }

        private async Task PullImageAsync(CancellationToken cancellationToken)
        {
            var createParameters = new ImagesCreateParameters
            {
                FromImage = this.settings.Image,
                //Tag = tag
            };
            var progress = new Progress<JSONMessage>(jsonMessage => { });
            await this.dockerClient.Images.CreateImageAsync(createParameters, null, progress, cancellationToken).ConfigureAwait(false);
        }

        private async Task<string> CreateContainerAsync(string networkId, CancellationToken token = default)
        {
            var createParameters = new CreateContainerParameters
            {
                Image = this.settings.Image,
                Env = this.settings.EnvVars,
                Name = $"{this.settings.Name}-{this.Identifier}",
                Hostname = this.settings.Name,
                HostConfig = new HostConfig
                {
                    PortBindings = PortBindings(this.settings.PortMappings),
                    NetworkMode = networkId,
                },

                ExposedPorts = this.settings.PortMappings != null ? this.settings.PortMappings.ToDictionary(x => x.Key, x => new EmptyStruct()) : null,
                Labels =  new Dictionary<string, string>() { { WellKnownLabels.InfraLabel, this.DomainName } },
            };

            var response = await this.dockerClient.Containers.CreateContainerAsync(createParameters, token).ConfigureAwait(false);
            return response.ID;
        }

        private static IDictionary<string, IList<PortBinding>> PortBindings(IDictionary<string, ISet<string>> portMappings)
        {
            if (portMappings == null)
            {
                return null;
            }

            return portMappings.Select(x => new
            {
                ContainerPort = x.Key,
                HostPorts = HostPorts(x.Value)
            })
            .ToDictionary(x => x.ContainerPort, x => (IList<PortBinding>)x.HostPorts);
        }

        private static List<PortBinding> HostPorts(IEnumerable<string> hostPorts)
        {
            if (hostPorts == null)
            {
                return null;
            }
            return hostPorts.Select(x => new PortBinding { HostPort = x }).ToList();
        }

        private static DirectoryInfo GetSolutionDirectory(DirectoryInfo dir)
        {
            if (dir.GetFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
            {
                return dir;
            }

            return GetSolutionDirectory(dir.Parent);
        }
    }
}
