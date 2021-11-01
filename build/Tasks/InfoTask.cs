using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Frosting;

[TaskName("info")]
public sealed class InfoTask : FrostingTask<BaseBuildContext>
{
    public override void Run(BaseBuildContext context)
    {
        context.DotNetCoreTool("--info");

        foreach (var variable in context.Environment.GetEnvironmentVariables())
        {
            context.Information("{0}:{1}", variable.Key, variable.Value);
        }
    }
}

