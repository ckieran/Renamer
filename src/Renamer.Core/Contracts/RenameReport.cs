using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenameReport
{
    [JsonPropertyName("schemaVersion")]
    public required string SchemaVersion { get; init; }

    [JsonPropertyName("planId")]
    public required string PlanId { get; init; }

    [JsonPropertyName("startedAtUtc")]
    public required string StartedAtUtc { get; init; }

    [JsonPropertyName("finishedAtUtc")]
    public required string FinishedAtUtc { get; init; }

    [JsonPropertyName("results")]
    public required IReadOnlyList<RenameReportResult> Results { get; init; }

    [JsonPropertyName("summary")]
    public required RenameReportSummary Summary { get; init; }
}
