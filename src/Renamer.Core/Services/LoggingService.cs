using System;
using Serilog;

namespace Renamer.Core.Services
{
    public class LoggingService : ILoggingService
    {
        private static bool _initialized = false;
        private static ILogger _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File("renamer.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

        public LoggingService()
        {
            // _logger is initialized at declaration; keep the flag for compatibility
            _initialized = true;
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message, Exception ex)
        {
            if (ex != null)
                _logger.Error(ex, message);
            else
                _logger.Error(message);
        }
    }
}