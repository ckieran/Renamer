using Renamer.Core.Logging;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Renamer.UI.Runtime;
using Microsoft.Extensions.DependencyInjection;
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
		builder.Services.AddSingleton<IPlanSerializer, PlanSerializer>();
		builder.Services.AddSingleton<IPlanFilePicker, PlanFilePicker>();
		builder.Services.AddSingleton<IRootPathOpener, RootPathOpener>();
		builder.Services.AddSingleton<IPlanViewModel, PlanViewModel>();
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
