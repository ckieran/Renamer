using System.Text.Json;
using Renamer.Core.Contracts;

namespace Renamer.Core.Serialization;

public sealed class ReportSerializer : IReportSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public void Write(string outputPath, RenameReport report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentNullException.ThrowIfNull(report);

        var normalizedReport = NormalizeReport(report);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(normalizedReport, SerializerOptions);
        File.WriteAllText(outputPath, json);
    }

    private static RenameReport NormalizeReport(RenameReport report)
    {
        var summary = new RenameReportSummary
        {
            Success = report.Results.Count(result => result.Status == "success"),
            Failed = report.Results.Count(result => result.Status == "failed"),
            Skipped = report.Results.Count(result => result.Status == "skipped"),
            Drifted = report.Results.Count(result =>
                result.Status == "success" &&
                !string.Equals(result.PlannedDestinationPath, result.ActualDestinationPath, StringComparison.Ordinal))
        };

        ValidateReport(report, summary);

        return new RenameReport
        {
            Outcome = report.Outcome,
            SchemaVersion = report.SchemaVersion,
            PlanId = report.PlanId,
            StartedAtUtc = report.StartedAtUtc,
            FinishedAtUtc = report.FinishedAtUtc,
            Results = report.Results,
            Summary = summary
        };
    }

    private static void ValidateReport(RenameReport report, RenameReportSummary summary)
    {
        if (string.IsNullOrWhiteSpace(report.Outcome))
            throw new InvalidOperationException("Invalid report invariant: outcome is required.");
        if (report.Results.Any(result => result.Attempts < 1))
            throw new InvalidOperationException("Invalid report invariant: each result.attempts must be at least 1.");

        var totalCount = summary.Success + summary.Failed + summary.Skipped;
        if (totalCount != report.Results.Count)
            throw new InvalidOperationException(
                $"Invalid report invariant: summary counts '{totalCount}' do not match results count '{report.Results.Count}'.");
    }
}
