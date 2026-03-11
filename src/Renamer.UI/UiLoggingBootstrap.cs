using Renamer.Core.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Renamer.UI;

public sealed class UiLoggingBootstrap
{
    private readonly ILogPathProvider logPathProvider;

    public UiLoggingBootstrap(ILogPathProvider logPathProvider)
    {
        this.logPathProvider = logPathProvider ?? throw new ArgumentNullException(nameof(logPathProvider));
    }

    public Serilog.Core.Logger CreateLogger()
    {
        var logFilePath = logPathProvider.GetLogFilePath("renamer-ui");
        return new LoggerConfiguration()
            .Enrich.WithProperty("Executable", "renamer-ui")
            .WriteTo.Console()
            .WriteTo.File(logFilePath, shared: true)
            .CreateLogger();
    }

    public void RegisterUnhandledExceptionLogging(Microsoft.Extensions.Logging.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                logger.LogCritical(exception, "Unhandled app domain exception in UI process.");
                return;
            }

            logger.LogCritical("Unhandled app domain exception in UI process.");
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            logger.LogCritical(args.Exception, "Unobserved task exception in UI process.");
        };
    }
}
