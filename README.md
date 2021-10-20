# Containerized Infrastructure

![](https://s.gravatar.com/avatar/c9683b06e0d615c2b661f03b26b51059?s=80)


| Badges  |    |
| ---------- | ------- |
| Activity   | [![GitHub](https://img.shields.io/github/last-commit/containerized-infrastructure/develop)](https://github.com/To-Do-Later/containerized-infrastructure) |
| build    | [![Build Status]()](https://travis-ci.org/joemccann/dillinger) |
| License    | [![GitHub](https://img.shields.io/github/license/containerized-infrastructure/readme.svg)](https://github.com/To-Do-Later/containerized-infrastructure) |

The motivation behind this project arise when we started to deal with too many ways of using container for integrations tests, we know that everyone has its preference (powershell, bash, docker compose...), but preferences aside, on the end of the day we only want to focus on the important part **just be able to run the integrations tests**.

## Initial requirements

- A simple setup builder of creating an infrastructure for the respective test,
- Support local and remote images,
- Provide an ease way to set up the evaluation of readiness of the containerized service,
- Provide an ease way to set up configuration\seed of the containerized service,
- Automatic clean up of the infrastructure.


## What's next?

We intend to create a level of abstraction that is agnostic to the container platform.
It's not just for the fun, the main reason is to provide the base that allow us to still use the platform of or preference (docker, kubernetes, podman,...) and still be compatible with our CI without the need of having different scripts for setup.


## Requirements

- Decoupling of the container platform,
- Provide out of the box support for generating valid and expired certificates.
- Add base functionality that allow an "ease" way to replicate infrastructure issues (service restarts, database deadlocks, network availability issues).


| NuGet Package| Version|Download|
| ----- | ----| --------- |
| ToDoLater.Containerized.Infrastructure.Docker| [![NuGet](https://img.shields.io/nuget/v/ToDoLater.Containerized.Infrastructure.Docker)](https://www.nuget.org/packages/ToDoLater.Containerized.Infrastructure.Docker) | [![Nuget](https://img.shields.io/nuget/dt/ToDoLater.Containerized.Infrastructure.Docker)](https://www.nuget.org/packages/Z.Blazor.Diagrams.Core) |


### Readiness definition

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
            // while we are not able to establish a connection and while the cancellationToken has not expired the system will continue on try. 
            return false;
        }
    }
}
```

### Set up definition

```csharp
public class SqlContainerizedFixture : DockerContainerizedService
{
    ...

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

### Configure infrastructure integration test fixture 

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
### Sample project

Repository: https://github.com/To-Do-Later/containerized-infrastructure/tree/master/examples/TestProject

### Usage on a normal console app

```cs
using (var tokenSource = new CancellationTokenSource(180000))
{
    await using (var infra = ContainerizedInfrastructureBuilder.UsingDocker
        .With<SqlService>(() => new DockerServiceSettings("db2", "mcr.microsoft.com/mssql/server:2017-latest", envVars: new List<string> { "ACCEPT_EULA=Y", "SA_PASSWORD=!MyMagicPasswOrd", "MSSQL_PID=Developer", "CHECK_POLICY=OFF", "CHECK_EXPIRY=OFF" }))
        .With<LocalApiService>(() => new DockerServiceSettings("webapi", dockerfilePath: @"../../../../../examples/sample/WebApplication/Dockerfile", tag:"Ab.GG"))
        .Build(new DockerInfrastructureSettings(new Uri("npipe://./pipe/docker_engine"), env)))
    {
        await infra.InitializeAsync(tokenSource.Token).ConfigureAwait(false);

        Console.WriteLine("Infra is Up!");

        await Task.Delay(20000).ConfigureAwait(false);
    }

    Console.WriteLine("Infra is down!");
}
```

### Sample project

Repository: https://github.com/To-Do-Later/containerized-infrastructure/tree/master/examples/ConsoleApp

</br>

## License

MIT

## Feedback

If you find a bug, or you want to see a functionality in this library, feel free to open an issue.