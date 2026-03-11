namespace Renamer.Core.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
