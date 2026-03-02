# 170 Plan JSON Writer

## Goal
Persist `rename-plan.json` from core plan models.

## In scope
- Write canonical plan artifact JSON.
- Populate required metadata and summary fields.
- Overwrite existing output file by default.

## Out of scope
- CLI argument parsing.
- Report generation.

## Implementation steps
1. Add `IPlanSerializer` and implementation in core.
2. Configure serializer options for stable JSON output:
   - camelCase property names
   - deterministic ordering by object model
   - include nulls only where schema allows nullable fields
3. Write plan artifact with overwrite semantics.
4. Validate required fields are serialized.
5. Add serializer tests against schema-required shape.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~PlanSerializer"`

## Acceptance checks
- Generated file validates against required field set.
- Operation count matches summary invariant.
- Existing output file is overwritten.

## Tests
- Add `src/Renamer.Tests/Core/PlanSerializerTests.cs`.

## Test scope
- Plan serialization and overwrite behavior.

## Expected outputs
- Plan serializer implementation.
- Plan serializer tests.

## Exit criteria
- Plan writer produces valid current contract output.
