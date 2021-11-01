using Cake.Frosting;

[TaskName("Default")]
[IsDependentOn(typeof(InfoTask))]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(RestoreTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
[IsDependentOn(typeof(PackTask))]
[IsDependentOn(typeof(PublishTask))]
[IsDependentOn(typeof(TaggingTask))]
public class DefaultTask : FrostingTask
{
}

