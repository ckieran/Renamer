using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenamePlanOperation
{
    [JsonPropertyName("opId")]
    public required string OpId { get; init; }

    [JsonPropertyName("sourcePath")]
    public required string SourcePath { get; init; }

    [JsonPropertyName("plannedDestinationPath")]
    public required string PlannedDestinationPath { get; init; }

    [JsonPropertyName("reason")]
    public required RenamePlanReason Reason { get; init; }
}
