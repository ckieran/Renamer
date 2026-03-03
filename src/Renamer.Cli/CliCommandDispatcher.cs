using Serilog;

namespace Renamer.Cli;

public static class CliCommandDispatcher
{
    public static int Dispatch(string[] args, TextWriter? output = null)
    {
        output ??= Console.Out;

        if (args.Length == 0)
        {
            Log.Warning("No command provided.");
            WriteHelp(output);
            return 2;
        }

        var command = args[0].Trim().ToLowerInvariant();
        switch (command)
        {
            case "help":
            case "--help":
            case "-h":
                WriteHelp(output);
                return 0;
            case "plan":
            case "apply":
                Log.Information("Accepted CLI command {Command}. Command implementation is pending.", command);
                return 0;
            default:
                Log.Warning("Unsupported CLI command {Command}.", command);
                WriteHelp(output);
                return 2;
        }
    }

    private static void WriteHelp(TextWriter output)
    {
        output.WriteLine("Renamer CLI");
        output.WriteLine("Available commands:");
        output.WriteLine("  help                Show this help.");
        output.WriteLine("  plan --root <path> --out <path>");
        output.WriteLine("  apply --plan <path> --out <path>");
    }
}
