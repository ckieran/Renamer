using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Renamer.Core.Execution;
using Renamer.Core.Exif;
using Renamer.Core.IO;
using Renamer.Core.Logging;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.Core.Time;
using Renamer.UI.Plans;
using Renamer.UI.Runtime;
using Renamer.UI.Services;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Renamer.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var runtimeEnvironment = new RuntimeEnvironment();
        var logPathProvider = new UiLogPathProvider(runtimeEnvironment);
        var uiLogFilePath = logPathProvider.GetLogFilePath("renamer-ui");
        var loggingBootstrap = new UiLoggingBootstrap(logPathProvider);
        var logger = loggingBootstrap.CreateLogger();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.ClearProviders();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IRuntimeEnvironment>(runtimeEnvironment);
        builder.Services.AddSingleton<ILogPathProvider>(logPathProvider);
        builder.Services.AddSingleton(loggingBootstrap);
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<IExifMetadataReader, MetadataExtractorExifMetadataReader>();
        builder.Services.AddSingleton<IExifService, ExifService>();
        builder.Services.AddSingleton<IFolderDateRangeCalculator, FolderDateRangeCalculator>();
        builder.Services.AddSingleton<IFolderNameGenerator, FolderNameGenerator>();
        builder.Services.AddSingleton<IConflictRetryPolicy, ConflictRetryPolicy>();
        builder.Services.AddSingleton<IPlanBuilder, PlanBuilder>();
        builder.Services.AddSingleton<IPlanSerializer, PlanSerializer>();
        builder.Services.AddSingleton<IDirectoryMover, DirectoryMover>();
        builder.Services.AddSingleton<IApplyEngine, ApplyEngine>();
        builder.Services.AddSingleton(FolderPicker.Default);
        builder.Services.AddSingleton<IFolderPathPicker, FolderPathPicker>();
        builder.Services.AddSingleton<IPlanFilePicker, PlanFilePicker>();
        builder.Services.AddSingleton<IRootPathOpener, RootPathOpener>();
        builder.Services.AddSingleton<IPlanViewModel, PlanViewModel>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Logging.AddSerilog(logger, dispose: true);

        var app = builder.Build();
        var appLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Renamer.UI.MauiProgram");
        loggingBootstrap.RegisterUnhandledExceptionLogging(appLogger);
        appLogger.LogInformation("UI log file path resolved to {LogFilePath}.", uiLogFilePath);
        appLogger.LogInformation("UI startup complete.");
        return app;
    }
}
