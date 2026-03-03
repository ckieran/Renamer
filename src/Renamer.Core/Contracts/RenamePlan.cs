using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenamePlan
{
    [JsonPropertyName("schemaVersion")]
    public required string SchemaVersion { get; init; }

    [JsonPropertyName("planId")]
    public required string PlanId { get; init; }

    [JsonPropertyName("createdAtUtc")]
    public required string CreatedAtUtc { get; init; }

    [JsonPropertyName("rootPath")]
    public required string RootPath { get; init; }

    [JsonPropertyName("operations")]
    public required IReadOnlyList<RenamePlanOperation> Operations { get; init; }

    [JsonPropertyName("summary")]
    public required RenamePlanSummary Summary { get; init; }
}
