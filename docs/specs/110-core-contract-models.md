# 110 Core Contract Models

## Goal
Define v1 DTOs and namespaces in `Renamer.Core` for plan/report contracts.

## In scope
- Add contracts namespace `Renamer.Core.Contracts`.
- Add types:
  - `RenamePlan`
  - `RenamePlanOperation`
  - `RenamePlanReason`
  - `RenamePlanSummary`
  - `RenameReport`
  - `RenameReportResult`
  - `RenameReportSummary`

## Out of scope
- File IO serialization.
- CLI/UI wiring.

## Implementation steps
1. Create `src/Renamer.Core/Contracts` directory.
2. Add DTO classes with required fields from `060-plan-schema.md`.
3. Add nullable types for report `actualDestinationPath` and `error`.
4. Ensure naming and property casing match schema field names.
5. Add minimal test fixture objects in test project for serialization checks.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~CoreContractModels"`

## Acceptance checks
- All required schema fields have a matching DTO property.
- DTO names and namespace are exactly as specified.

## Tests
- Add `src/Renamer.Tests/Core/ContractModelTests.cs`.
- Verify plan/report DTO round-trip serialization shape in memory.

## Test scope
- `ContractModelTests` only for this slice.

## Expected outputs
- New contract DTO files in `src/Renamer.Core/Contracts`.
- New test file `src/Renamer.Tests/Core/ContractModelTests.cs`.

## Exit criteria
- Core DTOs compile and tests pass.

