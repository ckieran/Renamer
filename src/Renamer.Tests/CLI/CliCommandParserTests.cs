using Renamer.Cli.Commands;

namespace Renamer.Tests.CLI;

public sealed class CliCommandParserTests
{
    [Fact]
    public void Parse_NoArguments_ReturnsInvalidMissingCommand()
    {
        var result = CliCommandParser.Parse([]);

        Assert.Equal(CliCommandType.Invalid, result.Type);
        Assert.Equal(CliCommandParseError.MissingCommand, result.ParseError);
    }

    [Theory]
    [InlineData("help")]
    [InlineData("--help")]
    [InlineData("-h")]
    public void Parse_HelpAliases_ReturnHelpCommand(string token)
    {
        var result = CliCommandParser.Parse([token]);

        Assert.Equal(CliCommandType.Help, result.Type);
        Assert.Equal(CliCommandParseError.None, result.ParseError);
    }

    [Fact]
    public void Parse_UnsupportedCommand_ReturnsInvalidUnsupported()
    {
        var result = CliCommandParser.Parse(["wat"]);

        Assert.Equal(CliCommandType.Invalid, result.Type);
        Assert.Equal(CliCommandParseError.UnsupportedCommand, result.ParseError);
        Assert.Equal("wat", result.CommandText);
    }

    [Fact]
    public void Parse_PlanCommand_PreservesRemainingArguments()
    {
        var result = CliCommandParser.Parse(["plan", "--root", "/photos", "--out", "/tmp/rename-plan.json"]);

        Assert.Equal(CliCommandType.Plan, result.Type);
        Assert.Equal(["--root", "/photos", "--out", "/tmp/rename-plan.json"], result.Arguments);
    }
}
