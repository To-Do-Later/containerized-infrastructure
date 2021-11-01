using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.NuGet.Push;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

[TaskName("publishNuget")]
public sealed class PublishTask : BaseFrostingTask<BaseBuildContext>
{
    public override bool ShouldRun(BaseBuildContext context) => !context.BuildSystem().IsLocalBuild;

    public override void Run(BaseBuildContext context)
    {
        var settings = new DotNetCoreNuGetPushSettings()
        {
            ApiKey = context.CIVariables.NugetApiKey,
            Source = context.CIVariables.NugetSouce,
            SkipDuplicate = context.SkipDuplicatePackages
        };

        foreach (var item in context.GetFiles(context.ArtifactsDirectory.FullPath + "/**/*.nupkg"))
        {
            context.Information("Executing {0} for package {1}", GetTaskName(), item.GetFilename());
            context.DotNetCoreNuGetPush(item.FullPath, settings);
        }
    }
}

