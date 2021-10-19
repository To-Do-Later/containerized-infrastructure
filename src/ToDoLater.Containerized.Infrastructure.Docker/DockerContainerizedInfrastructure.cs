namespace ToDoLater.Containerized.Infrastructure.Docker
{
    using global::Docker.DotNet;
    using global::Docker.DotNet.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ToDoLater.Containerized.Infrastructure.Abstraction;
    using ToDoLater.Containerized.Infrastructure.Docker.Models;

    public class DockerContainerizedInfrastructure : ContainerizedInfrastructure
    {
        private IDockerClient dockerClient;
        private readonly DockerInfrastructureSettings dockerInfrastructureSettings;

        private string dockerNetworkId;


        internal DockerContainerizedInfrastructure(IDockerClient dockerClient, DockerInfrastructureSettings dockerInfrastructureSettings, ICollection<IContainerizedService> containerizedServices) : base(containerizedServices)
        {
            this.Identifier = $"{dockerInfrastructureSettings.Environment}-{Guid.NewGuid().ToString("N")}";
            this.dockerClient = dockerClient;
            this.dockerInfrastructureSettings = dockerInfrastructureSettings;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await CleanContainersAsync(cancellationToken).ConfigureAwait(false);

            await CleanNetworksAsync(cancellationToken).ConfigureAwait(false);

            this.dockerNetworkId = await this.CreateNetworkAsync(cancellationToken).ConfigureAwait(false);

            var servicesInitializeTasks = containerizedServices.Select(v => v.InitializeAsync(this.dockerNetworkId, this.Identifier, dockerInfrastructureSettings.Environment, cancellationToken));
            await Task.WhenAll(servicesInitializeTasks).ConfigureAwait(false);
        }

        private async Task CleanNetworksAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Start cleaning networks for environment :{this.dockerInfrastructureSettings.Environment}");

            var networksListParameters = new NetworksListParameters()
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"label", new Dictionary<string, bool> {{ $"{WellKnownLabels.InfraLabel}={this.dockerInfrastructureSettings.Environment}", true}}}
                }
            };

            var networks = await this.dockerClient.Networks.ListNetworksAsync(networksListParameters, cancellationToken).ConfigureAwait(false);
            foreach (var network in networks)
            {
                await this.dockerClient.Networks.DeleteNetworkAsync(network.ID, cancellationToken).ConfigureAwait(false);
                //Console.WriteLine($"Network {network.ID} deleted.");
            }
        }

        private async Task CleanContainersAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Start cleaning containers for environment :{this.dockerInfrastructureSettings.Environment}");
            var containersListParameters = new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"label", new Dictionary<string, bool> {{ $"{WellKnownLabels.InfraLabel}={this.dockerInfrastructureSettings.Environment}", true}}}
                }
            };

            var containers = await this.dockerClient.Containers.ListContainersAsync(containersListParameters, cancellationToken).ConfigureAwait(false);
            var conf = new ContainerRemoveParameters { Force = true, RemoveVolumes = true };
            foreach (var container in containers)
            {
                if (container.Status != "exit")
                {
                    await this.dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters(), cancellationToken).ConfigureAwait(false);
                }
                await this.dockerClient.Containers.RemoveContainerAsync(container.ID, conf, cancellationToken).ConfigureAwait(false);
                //Console.WriteLine($"Container {container.ID} deleted.");
            }
        }

        private async Task<string> CreateNetworkAsync(CancellationToken cancellationToken)
        {
            var networksCreateParameters = new NetworksCreateParameters
            {
                Name = this.Identifier,
                Driver = "bridge",
                Labels = new Dictionary<string, string> { { WellKnownLabels.InfraLabel, this.dockerInfrastructureSettings.Environment } }
            };

            var response = await dockerClient.Networks.CreateNetworkAsync(networksCreateParameters, cancellationToken).ConfigureAwait(false);
            
            return response.ID;
        }

    }
}
