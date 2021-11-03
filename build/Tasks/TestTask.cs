using Build.Models;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Frosting;

[TaskName("test")]
public sealed class TestTask : TestProjectTargetTask<BaseBuildContext>
{
    public override void ForEachProject(BaseBuildContext context, NetCoreProject project)
    {
        var setting = new DotNetCoreTestSettings()
        {
            Configuration = context.ReleaseConfiguration,
            Verbosity = context.DotNetCoreVerbosityLevel,
            ResultsDirectory = context.TestResultsDirectory,
            NoBuild = true,
            NoRestore = true,
            NoLogo = true,
            Blame = true,
            DiagnosticOutput = false,
            Loggers = { "trx" }

        };
        context.DotNetCoreTest(project.FullPath, setting);
    }
}

