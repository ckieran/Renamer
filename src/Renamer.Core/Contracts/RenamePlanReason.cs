using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenamePlanReason
{
    [JsonPropertyName("startDate")]
    public required string StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public required string EndDate { get; init; }

    [JsonPropertyName("filesConsidered")]
    public required int FilesConsidered { get; init; }

    [JsonPropertyName("filesSkippedMissingExif")]
    public required int FilesSkippedMissingExif { get; init; }
}
