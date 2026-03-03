using Renamer.Cli.Commands;
using Renamer.Cli.Logging;
using Renamer.Cli.Runtime;
using Serilog;

namespace Renamer.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        var logPathProvider = new CliLogPathProvider(new RuntimeEnvironment());
        CliLoggerBootstrap.Configure(logPathProvider, "renamer-cli");

        try
        {
            Log.Information("CLI startup complete.");
            return CliCommandDispatcher.Dispatch(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled fatal exception in CLI process.");
            return (int)ProcessExitCode.UnexpectedRuntimeError;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
