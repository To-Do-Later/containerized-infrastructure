namespace ToDoLater.Containerized.Infrastructure
{
    using global::Docker.DotNet;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ToDoLater.Containerized.Infrastructure.Abstraction;
    using ToDoLater.Containerized.Infrastructure.Docker;
    using ToDoLater.Containerized.Infrastructure.Docker.Models;

    public static partial class ContainerizedInfrastructureBuilder
    {
        public static ICollection<DockerServiceBuilderSettings> UsingDocker { get { return new List<DockerServiceBuilderSettings>(); } }

        public static ICollection<DockerServiceBuilderSettings> With<T>(this ICollection<DockerServiceBuilderSettings> services, Func<DockerServiceSettings> configFactory) where T : DockerContainerizedService
        {
            services.Add(new DockerServiceBuilderSettings(typeof(T), configFactory));
            return services;
        }

        public static IContainerizedInfrastructure Build(this ICollection<DockerServiceBuilderSettings> dockerServices, DockerInfrastructureSettings dockerInfrastructureSettings)
        {
            var dockerClient = (dockerInfrastructureSettings.Credentials == null ?
                new DockerClientConfiguration(dockerInfrastructureSettings.Uri) :
                new DockerClientConfiguration(dockerInfrastructureSettings.Uri, dockerInfrastructureSettings.Credentials)).CreateClient();


            return new DockerContainerizedInfrastructure(
                dockerClient,
                dockerInfrastructureSettings,
                dockerServices.Select(t => (IContainerizedService)Activator.CreateInstance(t.ServiceType, dockerClient, t.ConfigFactory.Invoke())).ToList()
            );
        }
    }
}
