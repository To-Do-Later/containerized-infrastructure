using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoLater.Containerized.Infrastructure;
using ToDoLater.Containerized.Infrastructure.Docker;
using ToDoLater.Containerized.Infrastructure.Docker.Models;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                int total = 1;
                var infras = new List<string>();

                for (int i = 0; i < total; i++)
                {
                    infras.Add($"Dev-{i}");
                }

                var tasks = infras.Select(s=> UseAsync(s));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task UseAsync(string env)
        {
            using (var tokenSource = new CancellationTokenSource(180000))
            {
                await using (var infra = ContainerizedInfrastructureBuilder.UsingDocker
                    .With<SqlService>(() => new DockerServiceSettings("db2", "mcr.microsoft.com/mssql/server:2017-latest", envVars: new List<string> { "ACCEPT_EULA=Y", "SA_PASSWORD=!MyMagicPasswOrd", "MSSQL_PID=Developer", "CHECK_POLICY=OFF", "CHECK_EXPIRY=OFF" }))
                    .With<LocalApiService>(() => new DockerServiceSettings("webapi", 
                    dockerfilePath: @"../../../../../examples/sample/WebApplication/Dockerfile", 
                    tag:"ab.gg", 
                    portMappings: new Dictionary<string, ISet<string>> { { "8125", new HashSet<string> { "80" } } }
                    ))
                    .Build(new DockerInfrastructureSettings(new Uri("npipe://./pipe/docker_engine"), env)))
                {
                    await infra.InitializeAsync(tokenSource.Token).ConfigureAwait(false);

                    Console.WriteLine("Infra is Up!");

                    await Task.Delay(20000).ConfigureAwait(false);
                }

                Console.WriteLine("Infra is down!");
            }
        }
    }

    public class LocalApiService : DockerContainerizedService
    {
        public LocalApiService(IDockerClient dockerClient, DockerServiceSettings settings)
            : base(dockerClient, settings)
        {
        }
    }

    public class SqlService : DockerContainerizedService
    {
        public SqlService(IDockerClient dockerClient, DockerServiceSettings settings)
            : base(dockerClient, settings)
        {
        }
    }
}
