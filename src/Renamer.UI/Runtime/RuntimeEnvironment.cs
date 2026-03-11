namespace Renamer.UI.Runtime;

public sealed class RuntimeEnvironment : IRuntimeEnvironment
{
    public RuntimePlatform Platform =>
        OperatingSystem.IsWindows() ? RuntimePlatform.Windows :
        OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS() ? RuntimePlatform.MacOS :
        RuntimePlatform.Other;

    public string? LocalApplicationDataPath =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string? HomeDirectoryPath =>
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}
