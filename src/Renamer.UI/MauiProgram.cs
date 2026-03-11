using Renamer.Core.Logging;
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
		var loggingBootstrap = new UiLoggingBootstrap(logPathProvider);
		var logger = loggingBootstrap.CreateLogger();

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<IRuntimeEnvironment>(runtimeEnvironment);
		builder.Services.AddSingleton<ILogPathProvider>(logPathProvider);
		builder.Services.AddSingleton(loggingBootstrap);
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog(logger, dispose: true);

		var app = builder.Build();
		var appLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Renamer.UI.MauiProgram");
		loggingBootstrap.RegisterUnhandledExceptionLogging(appLogger);
		appLogger.LogInformation("UI startup complete.");
		return app;
	}
}
