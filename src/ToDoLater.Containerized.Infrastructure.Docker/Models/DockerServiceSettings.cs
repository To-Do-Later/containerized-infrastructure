namespace ToDoLater.Containerized.Infrastructure.Docker.Models
{
    using System.Collections.Generic;
    using ToDoLater.Containerized.Infrastructure.Models;

    public class DockerServiceSettings : ServiceSettings
    {
        public DockerServiceSettings(string name, string image, IDictionary<string, ISet<string>> portMappings = null, IList<string> envVars = null)
            : base(name, image, portMappings, envVars)
        {
            this.RemoteImage = true;
        }

        public DockerServiceSettings(string name, string dockerfilePath, string tag, IDictionary<string, ISet<string>> portMappings = null, IList<string> envVars = null)
            : base(name, $"{tag}:dev", portMappings, envVars)
        {
            this.DockerfilePath = dockerfilePath;
            this.Tag = tag;
        }

        public bool RemoteImage { get; protected set; }

        public string DockerfilePath { get; protected set; }

        public string Tag { get; protected set; }
    }
}
