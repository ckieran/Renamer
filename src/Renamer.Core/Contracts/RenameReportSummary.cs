using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenameReportSummary
{
    [JsonPropertyName("success")]
    public required int Success { get; init; }

    [JsonPropertyName("failed")]
    public required int Failed { get; init; }

    [JsonPropertyName("skipped")]
    public required int Skipped { get; init; }

    [JsonPropertyName("drifted")]
    public required int Drifted { get; init; }
}
