using System.Text.Json;
using System.Text.Json.Serialization;
using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public sealed class PlanSerializer : IPlanSerializer
{
    private const string CurrentSchemaVersion = "1.0";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public RenamePlan Read(string inputPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);

        var json = File.ReadAllText(inputPath);
        var plan = JsonSerializer.Deserialize<RenamePlan>(json, SerializerOptions)
            ?? throw new InvalidDataException("Plan artifact did not contain a valid plan document.");

        try
        {
            ValidatePlan(plan);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidDataException(ex.Message, ex);
        }

        ValidateReadablePlan(plan);

        return plan;
    }

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

    private static void ValidateReadablePlan(RenamePlan plan)
    {
        if (!string.Equals(plan.SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal))
        {
            throw new NotSupportedException($"Unsupported plan schemaVersion '{plan.SchemaVersion}'.");
        }

        if (string.IsNullOrWhiteSpace(plan.PlanId))
        {
            throw new InvalidDataException("Plan artifact is missing required field 'planId'.");
        }

        if (string.IsNullOrWhiteSpace(plan.RootPath))
        {
            throw new InvalidDataException("Plan artifact is missing required field 'rootPath'.");
        }

        foreach (var operation in plan.Operations)
        {
            if (string.IsNullOrWhiteSpace(operation.SourcePath))
            {
                throw new InvalidDataException("Plan artifact contains an operation with empty 'sourcePath'.");
            }

            if (string.IsNullOrWhiteSpace(operation.PlannedDestinationPath))
            {
                throw new InvalidDataException("Plan artifact contains an operation with empty 'plannedDestinationPath'.");
            }
        }
    }
}
