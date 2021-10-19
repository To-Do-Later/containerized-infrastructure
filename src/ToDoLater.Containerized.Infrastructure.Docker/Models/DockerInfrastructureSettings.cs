namespace ToDoLater.Containerized.Infrastructure.Docker.Models
{
    using global::Docker.DotNet;
    using System;
    using System.Collections.Generic;

    public class DockerInfrastructureSettings
    {
        public DockerInfrastructureSettings(Uri uri, string environment = default, IList<string> Labels = null)
        {
            this.Uri = uri;
            this.Environment = environment;
            this.Labels = Labels;
        }

        public DockerInfrastructureSettings(Uri uri, Credentials credentials, string environment = default, IList<string> Labels = null)
        {
            this.Uri = uri;
            this.Credentials = credentials;
            this.Environment = environment;
            this.Labels = Labels;
        }

        public Uri Uri { get; protected set; }

        public Credentials Credentials { get; protected set; }

        public string Environment { get; protected set; }

        public IList<string> Labels { get; protected set; }
    }
}
