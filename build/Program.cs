using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseStartup<Start>()
            .UseSetup<BuildSetup>()
            .UseContext<BaseBuildContext>()
            .Run(args);
    }
}