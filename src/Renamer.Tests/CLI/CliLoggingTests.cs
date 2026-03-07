using Renamer.Cli;
using Renamer.Cli.Runtime;
using Renamer.Cli.Commands;
using Renamer.Core.Exif;
using Renamer.Core.Logging;
using Microsoft.Extensions.Logging;

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
    public void CliApplication_WritesStartupLogToConfiguredLogFile()
    {
        Directory.CreateDirectory(_tempDirectory);
        var logPath = Path.Combine(_tempDirectory, "renamer-cli.log");
        using var output = new StringWriter();
        using var errorOutput = new StringWriter();
        var runtime = new FakeRuntimeEnvironment(RuntimePlatform.MacOS, null, _tempDirectory);
        var app = new CliApplication(
            output,
            errorOutput,
            runtime,
            new FixedLogPathProvider(logPath));

        var exitCode = app.Run(["help"]);

        Assert.Equal((int)ProcessExitCode.Success, exitCode);
        Assert.True(File.Exists(logPath));
        var content = File.ReadAllText(logPath);
        Assert.Contains("CLI startup complete.", content);
    }

    [Fact]
    public void CliApplication_StartupIoFailure_ReturnsIoExitCode()
    {
        using var output = new StringWriter();
        using var errorOutput = new StringWriter();
        var runtime = new FakeRuntimeEnvironment(RuntimePlatform.MacOS, null, _tempDirectory);
        var app = new CliApplication(
            output,
            errorOutput,
            runtime,
            new ThrowingLogPathProvider(new IOException("disk unavailable")));

        var exitCode = app.Run(["help"]);

        Assert.Equal((int)ProcessExitCode.IoFailure, exitCode);
        Assert.Contains("disk unavailable", errorOutput.ToString());
    }

    [Fact]
    public void CliApplication_UnhandledCommandFailure_ReturnsUnexpectedRuntimeError()
    {
        Directory.CreateDirectory(_tempDirectory);
        var logPath = Path.Combine(_tempDirectory, "renamer-cli.log");
        using var output = new StringWriter();
        using var errorOutput = new StringWriter();
        var runtime = new FakeRuntimeEnvironment(RuntimePlatform.MacOS, null, _tempDirectory);
        var app = new CliApplication(
            output,
            errorOutput,
            runtime,
            new FixedLogPathProvider(logPath),
            new ThrowingCommandHandler(),
            new FakeExifMetadataReader());

        var exitCode = app.Run(["help"]);

        Assert.Equal((int)ProcessExitCode.UnexpectedRuntimeError, exitCode);
        Assert.True(File.Exists(logPath));
        var content = File.ReadAllText(logPath);
        Assert.Contains("Unhandled fatal exception in CLI process.", content);
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

    private sealed class FixedLogPathProvider(string logPath) : ILogPathProvider
    {
        public string GetLogFilePath(string executableName) => logPath;
    }

    private sealed class ThrowingLogPathProvider(Exception exception) : ILogPathProvider
    {
        public string GetLogFilePath(string executableName) => throw exception;
    }

    private sealed class ThrowingCommandHandler : ICliCommandHandler
    {
        public CommandResult Handle(CliCommand command) => throw new InvalidOperationException("boom");
    }

    private sealed class FakeExifMetadataReader : IExifMetadataReader
    {
        public ExifMetadataReadResult ReadCaptureDate(string filePath) => ExifMetadataReadResult.Missing();
    }

}
