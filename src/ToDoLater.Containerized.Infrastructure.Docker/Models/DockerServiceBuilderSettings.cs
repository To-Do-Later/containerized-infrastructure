namespace ToDoLater.Containerized.Infrastructure.Docker.Models
{
    using System;
    using ToDoLater.Containerized.Infrastructure.Models;

    public class DockerServiceBuilderSettings : ServiceBuilderSettings
    {
        public DockerServiceBuilderSettings(Type type, Func<ServiceSettings> configFactory) : base(type, configFactory) { }
    }
}
