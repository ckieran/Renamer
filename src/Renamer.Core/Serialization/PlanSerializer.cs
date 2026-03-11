using System.Text.Json;
using System.Text.Json.Serialization;
using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public sealed class PlanSerializer : IPlanSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void Write(string outputPath, RenamePlan plan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentNullException.ThrowIfNull(plan);

        ValidatePlan(plan);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(plan, SerializerOptions);
        File.WriteAllText(outputPath, json);
    }

    private static void ValidatePlan(RenamePlan plan)
    {
        if (plan.Operations.Count != plan.Summary.OperationCount)
        {
            throw new InvalidOperationException(
                $"Invalid plan invariant: operations count '{plan.Operations.Count}' does not match summary.operationCount '{plan.Summary.OperationCount}'.");
        }
    }
}
