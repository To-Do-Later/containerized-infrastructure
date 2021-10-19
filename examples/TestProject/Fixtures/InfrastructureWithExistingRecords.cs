using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Fixtures.Infrastructure;
using ToDoLater.Containerized.Infrastructure;
using ToDoLater.Containerized.Infrastructure.Abstraction;
using ToDoLater.Containerized.Infrastructure.Docker.Models;
using Xunit;

namespace TestProject.Fixtures
{
    public class InfrastructureWithExistingRecords : IAsyncLifetime
    {
        protected IContainerizedInfrastructure containerizedInfrastructure;

        protected TestServer testServer { get; private set; }

        public HttpClient httpClient { get; private set; }

        public InfrastructureWithExistingRecords()
        {
            this.containerizedInfrastructure = ContainerizedInfrastructureBuilder.UsingDocker
                    .With<SqlContainerizedFixture>(() => new DockerServiceSettings(
                        name : "db1", 
                        image: "mcr.microsoft.com/mssql/server:2017-latest", 
                        envVars: new List<string> { "ACCEPT_EULA=Y", "SA_PASSWORD=!MyMagicPasswOrd", "MSSQL_PID=Developer", "CHECK_POLICY=OFF", "CHECK_EXPIRY=OFF" },
                        portMappings: new Dictionary<string, ISet<string>> { { "1433", new HashSet<string> { "1433" } } }
                        ))
                    .Build(new DockerInfrastructureSettings(new Uri("npipe://./pipe/docker_engine"), "XunitTestEnv"));
        }

        public async Task DisposeAsync()
        {
            await this.containerizedInfrastructure.DisposeAsync();
            testServer.Dispose();
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task InitializeAsync()
        {
            using (var tokenSource = new CancellationTokenSource(60000))
            {
                await this.containerizedInfrastructure.InitializeAsync(tokenSource.Token).ConfigureAwait(false);

                var builder = WebHost.CreateDefaultBuilder()
                    .UseEnvironment("Development")
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        //Override the settings to bind the API to our Testing environment
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"ConnectionStrings:DefaultConnection", "Server=localhost,1433;Database=WeatherForecastdb;User Id=WeatherForecastUser;Password=MyPassword;MultipleActiveResultSets=true"},
                        });
                    })
                    .UseStartup<WebApplication.Startup>()
                    .UseSetting(WebHostDefaults.ApplicationKey,
                         typeof(WebApplication.Program).GetTypeInfo().Assembly
                             .FullName);

                this.testServer = new TestServer(builder);
                this.httpClient = this.testServer.CreateClient();
            }
        }
    }
}
