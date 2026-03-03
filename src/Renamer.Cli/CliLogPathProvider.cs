using Renamer.Cli.Runtime;
using Renamer.Core.Logging;

namespace Renamer.Cli;

public sealed class CliLogPathProvider(IRuntimeEnvironment runtimeEnvironment) : ILogPathProvider
{
    public string GetLogFilePath(string executableName)
    {
        if (string.IsNullOrWhiteSpace(executableName))
        {
            throw new ArgumentException("Executable name is required.", nameof(executableName));
        }

        var logFileName = $"{executableName}.log";
        var directory = ResolveLogDirectory(runtimeEnvironment.Platform);
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, logFileName);
    }

    private string ResolveLogDirectory(RuntimePlatform platform)
    {
        return platform switch
        {
            RuntimePlatform.Windows => ResolveWindowsLogDirectory(),
            RuntimePlatform.MacOS => ResolveMacLogDirectory(),
            _ => ResolveDefaultLogDirectory()
        };
    }

    private string ResolveWindowsLogDirectory()
    {
        var localAppData = runtimeEnvironment.LocalApplicationDataPath;
        if (string.IsNullOrWhiteSpace(localAppData))
        {
            throw new InvalidOperationException("Local application data path is not available.");
        }

        return Path.Combine(localAppData, "Renamer", "logs");
    }

    private string ResolveMacLogDirectory()
    {
        var homePath = runtimeEnvironment.HomeDirectoryPath;
        if (string.IsNullOrWhiteSpace(homePath))
        {
            throw new InvalidOperationException("User home directory is not available.");
        }

        return Path.Combine(homePath, "Library", "Logs", "Renamer");
    }

    private string ResolveDefaultLogDirectory()
    {
        var homePath = runtimeEnvironment.HomeDirectoryPath;
        if (string.IsNullOrWhiteSpace(homePath))
        {
            throw new InvalidOperationException("User home directory is not available.");
        }

        return Path.Combine(homePath, ".local", "state", "Renamer", "logs");
    }
}
