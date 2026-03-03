using System.Runtime.InteropServices;

namespace Renamer.Cli.Runtime;

public sealed class RuntimeEnvironment : IRuntimeEnvironment
{
    public RuntimePlatform Platform
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimePlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimePlatform.MacOS;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimePlatform.Linux;
            }

            return RuntimePlatform.Other;
        }
    }

    public string? LocalApplicationDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string? HomeDirectoryPath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}
