using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenameReportResult
{
    [JsonPropertyName("opId")]
    public required string OpId { get; init; }

    [JsonPropertyName("sourcePath")]
    public required string SourcePath { get; init; }

    [JsonPropertyName("plannedDestinationPath")]
    public required string PlannedDestinationPath { get; init; }

    [JsonPropertyName("actualDestinationPath")]
    public string? ActualDestinationPath { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("attempts")]
    public required int Attempts { get; init; }

    [JsonPropertyName("warnings")]
    public required IReadOnlyList<string> Warnings { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
