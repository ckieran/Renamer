namespace Renamer.Core.Logging;

public interface ILogPathProvider
{
    string GetLogFilePath(string executableName);
}
