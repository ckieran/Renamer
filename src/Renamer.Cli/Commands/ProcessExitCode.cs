namespace Renamer.Cli.Commands;

public enum ProcessExitCode
{
    Success = 0,
    ValidationFailure = 2,
    IoFailure = 3,
    InvalidOrUnsupportedPlanSchema = 4,
    ConflictRetryLimitReached = 5,
    UnexpectedRuntimeError = 6
}
