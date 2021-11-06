using Build.Models;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core;
using Cake.Frosting;

[TaskName("pack")]
public sealed class PackTask : SourceProjectTargetTask<BaseBuildContext>
{
    public override void ForEachProject(BaseBuildContext context, NetCoreProject project)
    {
        var settings = new DotNetCorePackSettings()
        {
            Configuration = context.ReleaseConfiguration,
            OutputDirectory = context.ArtifactsDirectory,
            NoRestore = true,
            NoLogo = true,
            NoBuild = true,
            Verbosity = context.DotNetCoreVerbosityLevel,
            ArgumentCustomization = args => args
                            .AppendProperty("RepositoryType", context.CIVariables.RepositoryType)
                            .AppendProperty("RepositoryBranch", context.CIVariables.Branch)
                            .AppendProperty("RepositoryCommit", context.CIVariables.SHACommit)
        };

        context.DotNetCorePack(project.FullPath, settings);
    }
}

