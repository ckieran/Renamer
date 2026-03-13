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
#if MACCATALYST
        ResolveMacCatalystHomeDirectoryPath();
#else
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#endif

#if MACCATALYST
    private static string? ResolveMacCatalystHomeDirectoryPath()
    {
        try
        {
            using var homeDirectory = new Foundation.NSString("~");
            return homeDirectory.ExpandTildeInPath()?.ToString();
        }
        catch
        {
            return Environment.GetEnvironmentVariable("HOME")
                ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
#endif
}
