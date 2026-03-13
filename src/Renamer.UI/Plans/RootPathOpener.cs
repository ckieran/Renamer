using System.Diagnostics;

namespace Renamer.UI.Plans;

public sealed class RootPathOpener : IRootPathOpener
{
    public Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Root path '{directoryPath}' does not exist.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        using var process = new Process
        {
            StartInfo = CreateStartInfo(directoryPath)
        };

        if (!process.Start())
        {
            throw new InvalidOperationException($"Unable to open directory '{directoryPath}'.");
        }

        return Task.CompletedTask;
    }

    private static ProcessStartInfo CreateStartInfo(string directoryPath)
    {
        if (OperatingSystem.IsWindows())
        {
            return new ProcessStartInfo("explorer.exe", Quote(directoryPath))
            {
                UseShellExecute = false
            };
        }

        if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            return new ProcessStartInfo("/usr/bin/open", Quote(directoryPath))
            {
                UseShellExecute = false
            };
        }

        throw new NotSupportedException("Opening the root folder is not supported on this platform.");
    }

    private static string Quote(string value) => $"\"{value}\"";
}
