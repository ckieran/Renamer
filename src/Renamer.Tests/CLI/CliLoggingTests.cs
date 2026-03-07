using Renamer.Cli;
using Renamer.Cli.Runtime;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Renamer.Tests.CLI;

public sealed class CliLoggingTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "renamer-cli-tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void CliLogPathProvider_Windows_UsesLocalAppDataPath()
    {
        var runtime = new FakeRuntimeEnvironment(
            RuntimePlatform.Windows,
            localApplicationDataPath: @"C:\Users\tester\AppData\Local",
            homeDirectoryPath: "/unused");

        var provider = new CliLogPathProvider(runtime);

        var actual = provider.GetLogFilePath("renamer-cli");

        var expected = Path.Combine(@"C:\Users\tester\AppData\Local", "Renamer", "logs", "renamer-cli.log");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CliLogPathProvider_MacOS_UsesLibraryLogsPath()
    {
        var homePath = Path.Combine(_tempDirectory, "home");
        var runtime = new FakeRuntimeEnvironment(
            RuntimePlatform.MacOS,
            localApplicationDataPath: null,
            homeDirectoryPath: homePath);

        var provider = new CliLogPathProvider(runtime);

        var actual = provider.GetLogFilePath("renamer-cli");

        var expected = Path.Combine(homePath, "Library", "Logs", "Renamer", "renamer-cli.log");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerilogLoggerFactory_WritesToLogFile()
    {
        Directory.CreateDirectory(_tempDirectory);
        var logPath = Path.Combine(_tempDirectory, "renamer-cli.log");
        using var serilogLogger = new LoggerConfiguration()
            .WriteTo.File(logPath, shared: true)
            .CreateLogger();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: false));
        var logger = loggerFactory.CreateLogger("Renamer.Tests.CLI");

        logger.LogInformation("test log entry");

        Assert.True(File.Exists(logPath));
        var content = File.ReadAllText(logPath);
        Assert.Contains("test log entry", content);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
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
