using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.IO;

namespace Renamer.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		// Configure Serilog early
		var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs");
		Directory.CreateDirectory(logPath);
		var logFile = Path.Combine(logPath, "renamer-.log");
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
			.CreateLogger();

		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog(Log.Logger);
		
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.Services.AddSingleton<Renamer.UI.Services.IThemeService, Renamer.UI.Services.ThemeService>()
			// Core services
			.AddSingleton<Renamer.Core.Services.IExifService, Renamer.Core.Services.ExifService>()
			.AddSingleton<Renamer.Core.Services.IFileSystemService, Renamer.Core.Services.FileSystemService>()
			.AddSingleton<Renamer.Core.Services.IRenameService, Renamer.Core.Services.RenameService>()
			// UI pages (transient)
			.AddTransient<Renamer.UI.Views.FolderPickerPage>()
			.AddTransient<Renamer.UI.Views.PlanViewPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
