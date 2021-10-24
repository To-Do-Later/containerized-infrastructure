[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<br />
<div align="center">
  <a href="https://github.com/To-Do-Later/containerized-infrastructure">
    <img width="160px" height="160px" src="https://s.gravatar.com/avatar/c9683b06e0d615c2b661f03b26b51059?s=220">
  </a>

  <h1 align="center">Containerized Infrastructure</h1>

  <p align="center">
    A simple builder for creating your integration test infrastructure.
    <br />
    <a href="https://github.com/To-Do-Later/containerized-infrastructure"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/To-Do-Later/containerized-infrastructure/tree/master/examples">Samples</a>
    ·
    <a href="https://github.com/To-Do-Later/containerized-infrastructure/issues">Request Feature</a>
  </p>
</div>


## About The Project


This project was created with the intention of making easier/generic the setup a infrastructure using containers for integration tests.

The initial version only has support for docker desktop, but we have no intinction on stopping there, the main goal is to be agnostic how the definition of the infrastructure is done either for local development and CI, without having to force one tool (docker desktop, minikube...) to everyone else.

[![NuGet](https://img.shields.io/nuget/v/ToDoLater.Containerized.Infrastructure.Docker)](https://www.nuget.org/packages/ToDoLater)
[![Nuget](https://img.shields.io/nuget/dt/ToDoLater.Containerized.Infrastructure.Docker)](https://www.nuget.org/packages/ToDoLater.Containerized.Infrastructure.Docker)

## Roadmap

- [x] Create a simple setup builder for defining the infrastructure.
  - [x] Support local and remote images.
- [x] Provide an overridable method for evaluation of the containerized readiness,
- [x] Provide an overridable method for preparation of the service containerized before being available to the infrastructure,
- [x] Provide an overridable method for build of the local images.
- [x] Automatic clean up of the infrastructure.
- [ ] Create documentation.
- [ ] Remove the need of the extension UsingDocker.
- [ ] Move user configs and tools settings to a config file.
- [ ] Use an environment variable to evaluate witch tool to use (for CI only).
- [ ] Provide out of the box support for generating valid and expired certificates.
- [ ] Add base functionality that allow an "ease" way to replicate infrastructure issues (service restarts, addition and removal of services nodes, network availability issues).

## Getting Started

Explore the [examples](https://github.com/To-Do-Later/containerized-infrastructure/tree/master/examples)

### Prerequisites


Add the package to your project
* Package Manager
  ```PM
  PM> Install-Package ToDoLater.Containerized.Infrastructure.Docker
  ```
* PackageReference
  ```PM
  <PackageReference Include="ToDoLater.Containerized.Infrastructure.Docker"/>
  ```

### Setup

 1. Create a class which inherits from DockerContainerizedService.
 2. Override the IsReadyAsync to define how the check for readiness should be done.
 3. If you need to prepared the service (add a user, create a database..) override the PrepareAsync.

```csharp
public class SqlContainerizedFixture : DockerContainerizedService
{
    ...

    public override async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var port = this.settings.PortMappings.First().Key;
            using (var conn = new SqlConnection($"Server=localhost,{port};User=sa;Password=!MyMagicPasswOrd;Timeout=5;TrustServerCertificate=true"))
            {
                await conn.OpenAsync();
                return true;
            }
        }
        catch (Exception)
        {
            // while we are not able to establish a connection and while the cancellationToken hasn't expired the system will continue to try after a short delay.
            return false;
        }
    }

    public override async Task<bool> PrepareAsync(CancellationToken cancellationToken)
    {
        var port = this.settings.PortMappings.First().Key;
        using (var conn = new SqlConnection($"Server=localhost,{port};User=sa;Password=!MyMagicPasswOrd;Timeout=5;TrustServerCertificate=true"))
        {
            await conn.OpenAsync();

            foreach (var query in Queries)
            {
                using (var command = new SqlCommand(query, conn))
                {
                        command.ExecuteNonQuery();
                }
            }

            return true;
        }
    }

    private static List<string> Queries = new List<string>
    {
        @"IF DB_ID('WeatherForecastdb') IS NULL 
            BEGIN CREATE DATABASE WeatherForecastdb; 
        END",

       ...
    };
}
```

4. Create a class with the test infrastructure setup.
   
```csharp
public class InfrastructureWithExistingRecords : IAsyncLifetime
{
    protected IContainerizedInfrastructure containerizedInfrastructure;
    ...

    public InfrastructureWithExistingRecords()
    {
        this.containerizedInfrastructure = ContainerizedInfrastructureBuilder.UsingDocker
            .With<SqlContainerizedFixture>(() => new DockerServiceSettings(
                name : "db1", 
                image: "mcr.microsoft.com/mssql/server:2017-latest", 
                envVars: new List<string> { "ACCEPT_EULA=Y", "SA_PASSWORD=!MyMagicPasswOrd", "MSSQL_PID=Developer", "CHECK_POLICY=OFF", "CHECK_EXPIRY=OFF" },
                portMappings: new Dictionary<string, ISet<string>> { { "1433", new HashSet<string> { "1433" } } }
                ))
            .With<LocalApiService>(() => new DockerServiceSettings("webapi", dockerfilePath: @"../../../../../examples/sample/WebApplication/Dockerfile", tag:"Ab.GG"))
            .Build(new DockerInfrastructureSettings(new Uri("npipe://./pipe/docker_engine"), "XunitTestEnv"));
    }

    public async Task DisposeAsync()
    {
        await this.containerizedInfrastructure.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync()
    {
        using (var tokenSource = new CancellationTokenSource(60000))
        {
            await this.containerizedInfrastructure.InitializeAsync(tokenSource.Token).ConfigureAwait(false);
            ...
        }
    }
}
```

5. Start creating your integration test.

```csharp
[Collection("InfrastructureWithExistingRecordsCollection")]
public class AndOtherControllerTest
{
    private readonly InfrastructureWithExistingRecords infrastructureWithExistingRecords;

    public AndOtherControllerTest(InfrastructureWithExistingRecords infrastructureWithExistingRecords)
    {
        this.infrastructureWithExistingRecords = infrastructureWithExistingRecords;
    }

    [Fact]
    public async Task GetAndOtherByIdAsync_WithValidId_ReturnsOkWithForecast()
    {
        // arrange
        var id = "1A2B3C";

        // act
        using (HttpResponseMessage response = await this.infrastructureWithExistingRecords.httpClient.GetAsync($"/AndOther/{id}"))
        {
            var result = await response.Content.ReadAsAsync<WebApplication.WeatherForecast>();

            // assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Id.ShouldBe(id);
        }
    }
}
```

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

## Feedback

If you find a bug, or you want to see a functionality in this library, feel free to open an issue.


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/To-Do-Later/containerized-infrastructure.svg?style=for-the-badge
[contributors-url]: https://github.com/To-Do-Later/containerized-infrastructure/graphs/contributors

[forks-shield]: https://img.shields.io/github/forks/To-Do-Later/containerized-infrastructure.svg?style=for-the-badge
[forks-url]: https://github.com/To-Do-Later/containerized-infrastructure/network/members

[stars-shield]: https://img.shields.io/github/stars/To-Do-Later/containerized-infrastructure.svg?style=for-the-badge
[stars-url]: https://github.com/To-Do-Later/containerized-infrastructure/stargazers

[issues-shield]: https://img.shields.io/github/issues/To-Do-Later/containerized-infrastructure.svg?style=for-the-badge
[issues-url]: https://github.com/To-Do-Later/containerized-infrastructure/issues

[license-shield]: https://img.shields.io/github/license/To-Do-Later/containerized-infrastructure.svg?style=for-the-badge
[license-url]: https://github.com/To-Do-Later/containerized-infrastructure/blob/master/LICENSE.txt



[nuget-shield]: https://img.shields.io/nuget/dt/ToDoLater.Containerized.Infrastructure.Docker
[nuget-url]: https://www.nuget.org/packages/ToDoLater.Containerized.Infrastructure.Docker

[packages]: https://www.nuget.org/packages/ToDoLater
[nuget-docker]: https://www.nuget.org/packages/ToDoLater.Containerized.Infrastructure.Docker/1.0.0-preview01