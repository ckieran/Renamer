namespace Renamer.UI.Plans;

public sealed record ApplyResultItem(
    string OpId,
    string SourceName,
    string SourcePath,
    string StatusText,
    string PlannedDestinationPath,
    string ActualDestinationPathText,
    string WarningText,
    string ErrorText);
