using Cake.Frosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Start : IFrostingStartup
{
    public void Configure(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddJsonFile("localCI.json", true);
          //  .AddCommandLine()

        IConfiguration configuration = builder.Build();

        services.AddScoped<IConfiguration>(_ => configuration);
        services.AddScoped<ICIVariables, GitHubCIVariables>();

    }

}

