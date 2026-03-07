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
        var application = new CliApplication(
            Console.Out,
            Console.Error,
            runtimeEnvironment,
            new CliLogPathProvider(runtimeEnvironment));
        return application.Run(args);
    }
}

public sealed class CliApplication(
    TextWriter output,
    TextWriter errorOutput,
    IRuntimeEnvironment runtimeEnvironment,
    ILogPathProvider logPathProvider,
    ICliCommandHandler? commandHandler = null,
    IExifMetadataReader? exifMetadataReader = null)
{
    public int Run(string[] args)
    {
        try
        {
            using var logger = CreateLogger();
            using var serviceProvider = BuildServiceProvider(logger);
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
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            errorOutput.WriteLine($"CLI startup failed: {ex.Message}");
            return (int)ProcessExitCode.IoFailure;
        }
        catch (Exception ex)
        {
            errorOutput.WriteLine($"CLI startup failed: {ex.Message}");
            return (int)ProcessExitCode.UnexpectedRuntimeError;
        }
    }

    private Serilog.Core.Logger CreateLogger()
    {
        var logFilePath = logPathProvider.GetLogFilePath("renamer-cli");
        return new LoggerConfiguration()
            .Enrich.WithProperty("Executable", "renamer-cli")
            .WriteTo.Console()
            .WriteTo.File(logFilePath, shared: true)
            .CreateLogger();
    }

    private ServiceProvider BuildServiceProvider(Serilog.Core.Logger logger)
    {
        var services = new ServiceCollection();
        services.AddSingleton(runtimeEnvironment);
        services.AddSingleton(logPathProvider);
        services.AddSingleton(output);
        services.AddSingleton(commandHandler ?? new CliCommandHandler());
        services.AddSingleton(exifMetadataReader ?? new MetadataExtractorExifMetadataReader());
        services.AddSingleton<IExifService, ExifService>();
        services.AddSingleton<CliCommandDispatcher>();
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(logger, dispose: false);
        });

        return services.BuildServiceProvider();
    }
}
