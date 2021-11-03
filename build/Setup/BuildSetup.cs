using Cake.Common.IO;
using Cake.Core;
using Cake.Frosting;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using Cake.Core.IO;
using System.Collections.Generic;
using Build.Models;
using System;
using Build;

public class BuildSetup : FrostingSetup<BaseBuildContext>
{
    public override void Setup(BaseBuildContext context)
    {
        context.SetWorkingDirectory("../");
        context.SourceProjects = GetProjects(context, context.SourceDirectory.FullPath + "/**/*.csproj");
        context.TestsProjects = GetProjects(context, context.TestsDirectory.FullPath + "/**/*.csproj");
        context.Projects = context.SourceProjects.Union(context.TestsProjects);
    }

    private IEnumerable<NetCoreProject> GetProjects(BaseBuildContext context, GlobPattern pattern)
    {
        var projects = new List<NetCoreProject>();
        foreach (var project in context.GetFiles(pattern))
        {
            projects.Add(CsProjectParser.Parse(project.FullPath));
        }

        return projects.AsEnumerable();
    }


}

