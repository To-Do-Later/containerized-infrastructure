using Build.Models;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

public class BaseBuildContext : FrostingContext
{
    public IEnumerable<NetCoreProject> SourceProjects { get; set; }
    public IEnumerable<NetCoreProject> TestsProjects { get; set; }
    public FilePathCollection Artifacts { get; protected set; }
    public DirectoryPath SourceDirectory { get; protected set; }
    public DirectoryPath TestsDirectory { get; protected set; }
    public DirectoryPath ArtifactsDirectory { get; protected set; }
    public ICIVariables CIVariables { get; private set; }
    public string ReleaseConfiguration { get; internal set; }
    public DotNetCoreVerbosity? DotNetCoreVerbosityLevel { get; internal set; }
    public bool SkipDuplicatePackages { get; internal set; }
    public bool SkipDuplicateTags { get; internal set; }
    public DirectoryPath TestResultsDirectory { get; internal set; }
    public IEnumerable<NetCoreProject> Projects { get; internal set; }
    public string TagPrefix { get; set; }
    public object ReferenceVersion { get; internal set; }

    public BaseBuildContext(ICakeContext context, ICIVariables cIVariables, IConfiguration configuration)
        : base(context)
    {
        ArtifactsDirectory = configuration["ArtifactsDirectory"];
        SourceDirectory = configuration["SourceDirectory"];
        TestsDirectory = configuration["TestsDirectory"];
        TestResultsDirectory = configuration["TestResultsDirectory"];
        CIVariables = cIVariables;
        DotNetCoreVerbosityLevel = Enum.Parse<DotNetCoreVerbosity>(configuration["DotNetCoreVerbosityLevel"]);
        ReleaseConfiguration = configuration["ReleaseConfiguration"];
        SkipDuplicatePackages = bool.Parse(configuration["SkipDuplicatePackages"]);
        SkipDuplicateTags = bool.Parse(configuration["SkipDuplicateTags"]);
        TagPrefix = configuration["TagPrefix"];
        ReferenceVersion = configuration["ReferenceVersion"];
    }

    public void SetWorkingDirectory(string relativePath)
    {
        Environment.WorkingDirectory = new DirectoryPath(relativePath).MakeAbsolute(Environment.WorkingDirectory);
    }
}

