namespace ToDoLater.Containerized.Infrastructure.Models
{
    using System;

    public class ServiceBuilderSettings
    {
        public ServiceBuilderSettings(Type type, Func<ServiceSettings> configFactory)
        {
            this.ServiceType = type;
            this.ConfigFactory = configFactory;
        }

        public Type ServiceType { get; }

        public Func<ServiceSettings> ConfigFactory { get; }
    }
}
