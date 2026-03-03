using Renamer.Core.Logging;
using Serilog;

namespace Renamer.Cli.Logging;

public static class CliLoggerBootstrap
{
    public static void Configure(ILogPathProvider logPathProvider, string executableName)
    {
        var logFilePath = logPathProvider.GetLogFilePath(executableName);

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Executable", executableName)
            .WriteTo.Console()
            .WriteTo.File(logFilePath, shared: true)
            .CreateLogger();
    }
}
