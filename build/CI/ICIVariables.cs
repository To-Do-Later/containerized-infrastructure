public interface ICIVariables
{
    string SHACommit { get; }
    string RepositoryType { get; }
    string Branch { get; }
    string NugetApiKey { get; }
    string NugetSymbolApiKey { get; }
    string NugetSouce { get; }
}