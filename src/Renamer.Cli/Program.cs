using Renamer.Cli.Commands;
using Renamer.Cli.Runtime;
using Renamer.Core.Exif;
using Renamer.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Renamer.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        var runtimeEnvironment = new RuntimeEnvironment();
        ILogPathProvider logPathProvider = new CliLogPathProvider(runtimeEnvironment);
        var logger = new LoggerConfiguration()
            .Enrich.WithProperty("Executable", "renamer-cli")
            .WriteTo.Console()
            .WriteTo.File(logPathProvider.GetLogFilePath("renamer-cli"), shared: true)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton<IRuntimeEnvironment>(runtimeEnvironment);
        services.AddSingleton(logPathProvider);
        services.AddSingleton<TextWriter>(_ => Console.Out);
        services.AddSingleton<ICliCommandHandler, CliCommandHandler>();
        services.AddSingleton<IExifMetadataReader, MetadataExtractorExifMetadataReader>();
        services.AddSingleton<IExifService, ExifService>();
        services.AddSingleton<CliCommandDispatcher>();
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(logger, dispose: true);
        });

        using var serviceProvider = services.BuildServiceProvider();
        var appLogger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("Renamer.Cli.Program");

        try
        {
            appLogger.LogInformation("CLI startup complete.");
            return serviceProvider.GetRequiredService<CliCommandDispatcher>().Dispatch(args);
        }
        catch (Exception ex)
        {
            appLogger.LogCritical(ex, "Unhandled fatal exception in CLI process.");
            return (int)ProcessExitCode.UnexpectedRuntimeError;
        }
    }
}
