using Build.Models;
using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using System.Collections.Generic;

public abstract class SourceProjectTargetTask<T> : ProjectTargetTask<T> where T : BaseBuildContext
{
    public override IEnumerable<NetCoreProject> GetProjects(T context) => context.SourceProjects;
}

