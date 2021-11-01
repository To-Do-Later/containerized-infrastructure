using Cake.Common.Tools.DotNetCore;
using Cake.Frosting;

[TaskName("info")]
public sealed class InfoTask : FrostingTask<BaseBuildContext>
{
    public override void Run(BaseBuildContext context)
    {
        context.DotNetCoreTool("--info");
    }
}

