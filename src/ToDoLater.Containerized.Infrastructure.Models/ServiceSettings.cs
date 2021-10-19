namespace ToDoLater.Containerized.Infrastructure.Models
{
    using System.Collections.Generic;

    public class ServiceSettings
    {
        public ServiceSettings(string name, string image, IDictionary<string, ISet<string>> portMappings = null, IList<string> envVars = null)
        {
            this.Name = name;
            this.Image = image;
            this.PortMappings = portMappings;
            this.EnvVars = envVars;
        }

        public string Name { get; protected set; }

        public IDictionary<string, ISet<string>> PortMappings { get; protected set; }

        public string Image { get; protected set; }

        public IList<string> EnvVars { get; protected set; }
    }
}
