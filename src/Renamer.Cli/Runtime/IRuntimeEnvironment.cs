namespace Renamer.Cli.Runtime;

public interface IRuntimeEnvironment
{
    RuntimePlatform Platform { get; }

    string? LocalApplicationDataPath { get; }

    string? HomeDirectoryPath { get; }
}
