using Build.Models;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Frosting;

[TaskName("restore")]
public sealed class RestoreTask : ProjectTargetTask<BaseBuildContext>
{
    public override void ForEachProject(BaseBuildContext context, NetCoreProject project)
    {
        var setting = new DotNetCoreRestoreSettings()
        {
            Verbosity = context.DotNetCoreVerbosityLevel
        };
        context.DotNetCoreRestore(project.FullPath, setting);
    }
}

