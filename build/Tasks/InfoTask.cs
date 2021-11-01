using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName("info")]
public sealed class InfoTask : FrostingTask<BaseBuildContext>
{
    public override void Run(BaseBuildContext context)
    {
        context.DotNetCoreTool("--info");

        if (context.Log.Verbosity == Verbosity.Diagnostic)
        {
            foreach (var variable in context.Environment.GetEnvironmentVariables())
            {
                context.Verbose("{0}:{1}", variable.Key, variable.Value);
            }
        }

    }
}

