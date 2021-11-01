using Build.Models;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Core;
using Cake.Frosting;

[TaskName("build")]
public sealed class BuildTask : ProjectTargetTask<BaseBuildContext>
{
    public override void ForEachProject(BaseBuildContext context, NetCoreProject project)
    {
        var setting = new DotNetCoreBuildSettings()
        {
            Configuration = context.ReleaseConfiguration,
            Verbosity = context.DotNetCoreVerbosityLevel,
            NoLogo = true,
            NoRestore = true,
            ArgumentCustomization = args => args
                .Append($"-consoleLoggerParameters:NoSummary") //to fix dotnet build vebosity bug (https://github.com/dotnet/sdk/issues/10032)
        };
        context.DotNetCoreBuild(project.FullPath, setting);
    }
}

