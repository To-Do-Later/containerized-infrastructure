using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Frosting;
using Cake.Git;
using System.Linq;

[TaskName("tagging")]
public sealed class TaggingTask : FrostingTask<BaseBuildContext>
{
    public override bool ShouldRun(BaseBuildContext context) => !context.BuildSystem().IsLocalBuild;

    public override void Run(BaseBuildContext context)
    {
        var version = context.ReferenceVersion;
        var tagName = string.Format("{0}{1}", context.TagPrefix, version);
        var tags = context.GitTags(".");

        if (!tags.Any(x => x.FriendlyName == tagName) || !context.SkipDuplicateTags)
        {
            context.GitTag(".", tagName);
            context.GitPushRef(".", "origin", tagName);
        }
        else
        {
            context.Warning("Tag {0} already exists", tagName);
        }


    }
}

