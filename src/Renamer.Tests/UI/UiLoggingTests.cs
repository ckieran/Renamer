using Renamer.UI;
using Renamer.UI.Runtime;

namespace Renamer.Tests.UI;

public sealed class UiLoggingTests
{
    private readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "renamer-ui-tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void UiLogPathProvider_Windows_UsesLocalAppDataPath()
    {
        var runtime = new FakeRuntimeEnvironment(
            RuntimePlatform.Windows,
            localApplicationDataPath: @"C:\Users\tester\AppData\Local",
            homeDirectoryPath: "/unused");
        var provider = new UiLogPathProvider(runtime);

        var actual = provider.GetLogFilePath("renamer-ui");

        var expected = Path.Combine(@"C:\Users\tester\AppData\Local", "Renamer", "logs", "renamer-ui.log");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UiLogPathProvider_MacOS_UsesLibraryLogsPath()
    {
        var homePath = Path.Combine(tempDirectory, "home");
        var runtime = new FakeRuntimeEnvironment(
            RuntimePlatform.MacOS,
            localApplicationDataPath: null,
            homeDirectoryPath: homePath);
        var provider = new UiLogPathProvider(runtime);

        var actual = provider.GetLogFilePath("renamer-ui");

        var expected = Path.Combine(homePath, "Library", "Logs", "Renamer", "renamer-ui.log");
        Assert.Equal(expected, actual);
    }

    private sealed class FakeRuntimeEnvironment(
        RuntimePlatform platform,
        string? localApplicationDataPath,
        string? homeDirectoryPath) : IRuntimeEnvironment
    {
        public RuntimePlatform Platform { get; } = platform;

        public string? LocalApplicationDataPath { get; } = localApplicationDataPath;

        public string? HomeDirectoryPath { get; } = homeDirectoryPath;
    }
}
