namespace Renamer.Cli.Commands;

public interface ICliCommandHandler
{
    CommandResult Handle(CliCommand command);
}
