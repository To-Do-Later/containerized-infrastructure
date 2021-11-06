using Microsoft.Extensions.Configuration;

public sealed class GitHubCIVariables : ICIVariables
{
    private readonly IConfiguration _configuration;

    public GitHubCIVariables(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string SHACommit => _configuration["GITHUB_SHA"];
    public string Branch => _configuration["GITHUB_HEAD_REF"];
    public string NugetApiKey => _configuration[_configuration["NugetApiKeyConfig"]];
    public string NugetSymbolApiKey => _configuration[_configuration["SymbolApiKeyConfig"]];
    public string NugetSouce => _configuration["NugetSouce"];
    public string RepositoryType => "git";
}

