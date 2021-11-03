using Build.Models;
using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using System.Collections.Generic;
using System.Linq;

public abstract class ProjectTargetTask<T> : BaseFrostingTask<T> where T : BaseBuildContext
{
    public override void Run(T context)
    {
        var projects = GetProjects(context);

        if (!projects.Any())
        {
            context.Warning("No project to execute {0}", GetTaskName());
            return;
        }

        foreach (var project in projects)
        {
            context.Information("Executing {0} for project {1}", GetTaskName(), project.Name);
            ForEachProject(context, project);
        }
    }

    public abstract void ForEachProject(T context, NetCoreProject project);

    public virtual IEnumerable<NetCoreProject> GetProjects(T context) => context.Projects;
}

