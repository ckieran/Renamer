# 190 Report JSON Writer

## Goal
Persist `rename-report.json` with full operation outcomes.

## In scope
- Write required top-level report fields.
- Include `sourcePath`, planned/actual destination, status, attempts, warnings, error.
- Compute summary counters including `drifted`.
- Overwrite existing output file by default.

## Out of scope
- UI rendering.
- Log formatting.

## Implementation steps
1. Add `IReportSerializer` and implementation in core.
2. Configure serializer options consistent with plan serializer.
3. Serialize report results with required nullable fields.
4. Compute and validate summary invariants before write.
5. Add tests for drift counting and overwrite behavior.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~ReportSerializer"`

## Acceptance checks
- Report includes required fields for each result.
- Summary invariants hold.
- Existing output file is overwritten.

## Tests
- Add `src/Renamer.Tests/Core/ReportSerializerTests.cs`.

## Test scope
- Report serialization and summary math.

## Expected outputs
- Report serializer implementation and tests.

## Exit criteria
- Report writer produces valid current contract output.
