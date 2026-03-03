using System.Text.Json.Serialization;

namespace Renamer.Core.Contracts;

public sealed class RenamePlanSummary
{
    [JsonPropertyName("operationCount")]
    public required int OperationCount { get; init; }

    [JsonPropertyName("warnings")]
    public required int Warnings { get; init; }
}
