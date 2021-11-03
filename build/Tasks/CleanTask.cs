using Build.Models;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Clean;
using Cake.Frosting;

[TaskName("clean")]
public sealed class CleanTask : ProjectTargetTask<BaseBuildContext>
{
    public override void Run(BaseBuildContext context)
    {
        context.CleanDirectory(context.ArtifactsDirectory);

        base.Run(context);
    }

    public override void ForEachProject(BaseBuildContext context, NetCoreProject project)
    {
        var setting = new DotNetCoreCleanSettings()
        {
            Verbosity = context.DotNetCoreVerbosityLevel,
            NoLogo = true,
            Configuration = context.ReleaseConfiguration
        };
        context.DotNetCoreClean(project.FullPath, setting);
    }
}