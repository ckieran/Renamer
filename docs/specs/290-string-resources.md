# String Resource Extraction

## Slice ID
`290-string-resources`

## Goal
Replace all user-facing hardcoded strings in the UI and CLI layers with `.resx` resource files, eliminating magic strings and establishing the infrastructure for future multi-language support.

## Background

The codebase currently contains ~130+ hardcoded strings across 20+ files. These fall into two distinct categories that require different treatment:

**User-facing strings (in scope)** — shown to the user at runtime via the UI or CLI stdout/stderr:
- XAML `Text` and `Placeholder` attribute values in all five view files (~60 strings)
- UI status messages, error titles, and error descriptions in `PlanViewModel.cs` (~30 strings)
- CLI help text and error output in `CliCommandHandler.cs` (~13 strings)

**Developer-facing strings (out of scope)** — never rendered to the end user:
- Structured logging templates (e.g. `LogInformation("Plan loaded {Count}"...)`) — these belong with the code that emits them; they are ops/developer facing and have structured-logging formatting constraints
- Internal exception messages on `InvalidDataException`, `InvalidOperationException`, etc. — these validate code invariants and should remain adjacent to the validation logic
- Test fixture strings

The `.resx` approach is chosen over a plain constants class because it solves both problems simultaneously: it generates a strongly-typed accessor class (eliminating magic strings) and it is the .NET standard mechanism for runtime locale switching (enabling i18n without future migration work).

## In scope

- Create `Renamer.UI/Resources/Strings/AppStrings.resx` as the neutral-language fallback for all UI-layer user-facing strings.
- Create `Renamer.Cli/Resources/CliStrings.resx` as the neutral-language fallback for all CLI-layer user-facing strings.
- Extract all XAML `Text` and `Placeholder` literal values to `AppStrings.resx` and replace with `{x:Static}` bindings.
- Extract all user-visible string literals from `PlanViewModel.cs` (status messages, error titles, error descriptions, workflow step titles and descriptions) to `AppStrings.resx` and reference via the generated accessor class.
- Extract all CLI output strings from `CliCommandHandler.cs` to `CliStrings.resx` and reference via the generated accessor class.
- Update the XML namespace declaration in all affected XAML files to import the strings namespace.
- Confirm the generated accessor classes are `internal` (default) and accessed only within their own project.
- Add an entry to the index (`000-index.md`) for this slice.

## Out of scope

- Structured logging message templates — these stay in source code adjacent to the logging calls.
- Internal exception messages on `InvalidDataException`, `InvalidOperationException`, `ArgumentException`, etc. — these stay in source code adjacent to the validation logic.
- Adding any second-language `.resx` file (e.g. `AppStrings.fr.resx`) — this slice only establishes the infrastructure; translations are a separate concern.
- Runtime locale switching UI — out of scope for this slice.
- `Renamer.Core` — the core library contains no user-facing strings; it has only logging templates and internal exception messages, both of which are out of scope.
- `Renamer.Tests` — test fixture strings stay in test code.

## Pre-implementation gate (must pass before code edits)

1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c codex/290-string-resources`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `codex/290-string-resources`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps

### 1. Create `AppStrings.resx` for the UI project

Create `src/Renamer.UI/Resources/Strings/AppStrings.resx` with build action `EmbeddedResource` and custom tool `ResXFileCodeGenerator`.

The file must cover the following string categories. Key names use `PascalCase` grouped by screen/context:

**Workflow rail:**
- `WorkflowRailHeader` — "Workflow"
- `WorkflowRailDescription` — "Select any step to work in that area. Status shows whether the step needs input, hit an error, or completed successfully."

**Workflow step definitions (used in PlanViewModel step initialisation):**
- `StepGenerateTitle` — "Generate Plan"
- `StepGenerateDescription` — "Create a folder rename plan starting from a source folder"
- `StepPreviewTitle` — "Preview Plan"
- `StepPreviewDescription` — "Load a plan artifact and inspect the planned operations."
- `StepApplyTitle` — "Apply Plan"
- `StepApplyDescription` — "Execute the loaded plan and review the resulting report."

**Generate workspace:**
- `GenerateHeading` — "Generate Plan"
- `GenerateDescription` — "Pick the source root and output folder, then generate a plan artifact for the rest of the workflow."
- `GenerateSectionHeader` — "Generate Plan"
- `GenerateInstructions` — "Select the source root folder and destination folder for the generated plan artifact."
- `GenerateLabelRootFolder` — "Root Folder"
- `GeneratePlaceholderRootFolder` — "/path/to/photo-folder"
- `GenerateBrowseRootFolder` — "Browse Root Folder"
- `GenerateLabelOutputFolder` — "Output Folder"
- `GeneratePlaceholderOutputFolder` — "/path/to/output-folder"
- `GenerateBrowseOutputFolder` — "Browse Output Folder"
- `GenerateLabelPlanFileName` — "Plan File Name"
- `GeneratePlaceholderPlanFileName` — "rename-plan.json"
- `GenerateLabelGeneratedPlanPath` — "Generated Plan Path"
- `GenerateButtonGenerateAndLoad` — "Generate and Load"

**Generate status/error messages (used in PlanViewModel):**
- `GenerateStatusDefault` — "Select a root folder and output location to generate a plan."
- `GenerateStatusRootCanceled` — "Root folder selection canceled."
- `GenerateStatusRootSelected` — "Selected root folder: {0}"  *(use `string.Format` with index placeholder)*
- `GenerateStatusOutputCanceled` — "Output folder selection canceled."
- `GenerateStatusOutputSelected` — "Selected output folder: {0}"
- `GenerateStatusInProgress` — "Generating rename plan..."
- `GenerateStatusSuccess` — "Plan generated: {0}"
- `GenerateStatusUnavailable` — "Plan generation unavailable."
- `GenerateErrorNoRootTitle` — "Plan generation requires a root folder"
- `GenerateErrorNoRootMessage` — "Select a source root folder before generating a plan."
- `GenerateErrorInvalidRootTitle` — "Plan generation requires a valid root folder"
- `GenerateErrorInvalidRootMessage` — "Root folder '{0}' does not exist."
- `GenerateErrorNoOutputTitle` — "Plan generation requires an output folder"
- `GenerateErrorNoOutputMessage` — "Select an output folder for rename-plan.json."
- `GenerateErrorNoFileNameTitle` — "Plan generation requires a file name"
- `GenerateErrorNoFileNameMessage` — "Enter a file name for the generated plan artifact."
- `GenerateErrorInvalidFileNameTitle` — "Plan file name is invalid"
- `GenerateErrorFileSystemTitle` — "Plan generation failed due to file system error"
- `GenerateErrorUnexpectedTitle` — "Plan generation failed unexpectedly"

**Generate folder picker dialog titles (passed to `IFolderPathPicker.PickFolderPathAsync`):**
- `GenerateFolderPickerRootTitle` — "Select photo root folder"
- `GenerateFolderPickerOutputTitle` — "Select output folder for rename-plan.json"

**Preview workspace:**
- `PreviewHeading` — "Preview Plan"
- `PreviewDescription` — "Load a plan artifact, review the summary, and inspect each planned rename operation."
- `PreviewSectionHeader` — "Plan Artifact"
- `PreviewInstructions` — "Select or paste a path to a rename-plan.json file."
- `PreviewPlaceholder` — "/path/to/rename-plan.json"
- `PreviewBrowse` — "Browse"
- `PreviewButtonLoad` — "Load Preview"
- `PreviewIdleLabel` — "Preview workspace"
- `PreviewIdleDescription` — "Select a plan artifact and load it here to review the summary first, then inspect each planned rename operation below."
- `PreviewLoadingLabel` — "Loading Preview"
- `PreviewErrorHeading` — "Unable To Load Plan"
- `PreviewSummarySectionHeader` — "Summary"
- `PreviewSummaryDescription` — "Review the loaded plan metadata before scanning through the planned rename operations."
- `PreviewFieldRootPath` — "Root Path"
- `PreviewFieldCreatedAt` — "Created At"
- `PreviewFieldOperations` — "Operations"
- `PreviewFieldWarnings` — "Warnings"
- `PreviewOperationsSectionHeader` — "Planned Operations"
- `PreviewOperationsTotal` — "{0} total"
- `PreviewOperationsDescription` — "Operations remain scrollable in the wider workspace so longer destination names and paths stay readable."

**Preview status/error messages:**
- `PreviewStatusDefault` — "Select a rename-plan.json file to preview planned operations."  *(initial `statusMessage` default)*
- `PreviewCreatedAtDefault` — "No plan loaded"  *(initial `createdAtDisplay` default)*
- `PreviewStatusPathUpdated` — "Plan path updated. Load preview to refresh."
- `PreviewStatusBrowseOpening` — "Opening plan file picker..."
- `PreviewStatusBrowseCanceled` — "Plan selection canceled."
- `PreviewStatusBrowseSelected` — "Selected plan artifact: {0}"
- `PreviewStatusBrowseError` — "Unable to select a plan artifact: {0}"
- `PreviewStatusRootOpened` — "Opened root folder."
- `PreviewStatusRootError` — "Unable to open root folder: {0}"
- `PreviewStatusNoPath` — "Select a plan artifact path to load."
- `PreviewStatusLoading` — "Loading plan preview..."
- `PreviewStatusLoadError` — "Unable to load plan artifact: {0}"
- `PreviewStatusLoaded` — "Loaded {0} planned operation(s)."
- `PreviewStatusUnavailable` — "Plan preview unavailable."

**Apply workspace:**
- `ApplyHeading` — "Apply Plan"
- `ApplyDescription` — "Load a plan artifact, then run apply and inspect the resulting report summary and per-operation outcomes."
- `ApplyPlanArtifactSectionHeader` — "Plan Artifact"
- `ApplyPlanArtifactInstructions` — "Select or paste a path to a rename-plan.json file. The most recently generated plan is prepopulated automatically."
- `ApplyButtonBrowse` — "Browse"
- `ApplyButtonLoadPlan` — "Load Plan"
- `ApplyErrorHeading` — "Unable To Load Plan"  *(static label shown when `HasError` is true)*
- `ApplySectionHeader` — "Apply"
- `ApplyInstructions` — "Run apply against the loaded plan artifact. Results appear inline below."
- `ApplyButtonRun` — "Run Apply"
- `ApplySummarySectionHeader` — "Apply Summary"
- `ApplyFieldOutcome` — "Outcome"
- `ApplyFieldDrifted` — "Drifted"
- `ApplyFieldStarted` — "Started"
- `ApplyFieldFinished` — "Finished"
- `ApplyFieldSuccess` — "Success"
- `ApplyFieldSkipped` — "Skipped"
- `ApplyFieldFailed` — "Failed"
- `ApplyResultsSectionHeader` — "Apply Results"
- `ApplyFieldPlannedDestination` — "Planned Destination"
- `ApplyFieldActualDestination` — "Actual Destination"
- `ApplyFieldResultWarnings` — "Warnings"
- `ApplyFieldError` — "Error"

**Apply status/error messages:**
- `ApplyStatusDefault` — "Load a plan preview to enable apply."
- `ApplyStatusOutcomeDefault` — "Not run"
- `ApplyStatusStartedDefault` — "Not run"
- `ApplyStatusFinishedDefault` — "Not run"
- `ApplyStatusInProgress` — "Applying rename plan..."
- `ApplyStatusPartial` — "Apply stopped before the full plan completed."
- `ApplyStatusSuccess` — "Apply completed with {0} success, {1} skipped, {2} failed."
- `ApplyStatusUnavailable` — "Apply unavailable."

**Apply error titles (set on `ApplyErrorTitle` ViewModel property):**
- `ApplyErrorTitleRetryAbort` — "Apply stopped after conflict retry limit"
- `ApplyErrorTitleInvalidPlan` — "Plan artifact is invalid"
- `ApplyErrorTitleFileSystem` — "Apply failed due to file system error"
- `ApplyErrorTitleUnexpected` — "Apply failed unexpectedly"
- `ApplyErrorTitleValidation` — "Apply validation failed"

**Apply error message bodies (set on `ApplyErrorMessage` ViewModel property):**
- `ApplyErrorMessageValidation` — "Select and load a valid plan artifact before apply."
- `ApplyErrorMessageInvalidPlan` — "Unable to apply the selected plan artifact: {0}"
- `ApplyErrorMessageFileSystem` — "Unable to complete apply: {0}"

**Main page:**
- `MainPageHeading` — "Plan Generation, Preview, and Apply"
- `MainPageDescription` — "Move between generation, preview, and apply at any time. Each step keeps its own status while the active workspace fills the right panel."

### 2. Create `CliStrings.resx` for the CLI project

Create `src/Renamer.Cli/Resources/CliStrings.resx` with build action `EmbeddedResource` and custom tool `ResXFileCodeGenerator`.

**Help output:**
- `HelpHeader` — "Renamer CLI"
- `HelpCommandsLabel` — "Available commands:"
- `HelpCommandHelp` — "  help                Show this help."
- `HelpCommandPlan` — "  plan --root <path> --out <path>"
- `HelpCommandApply` — "  apply --plan <path> --out <path>"

**Plan command errors:**
- `PlanErrorMissingArgs` — "Error: Missing required arguments for 'plan'. Expected --root <path> and --out <path>."
- `PlanErrorRootNotFound` — "Root path '{0}' does not exist or is not a directory."
- `PlanErrorOutputIsDirectory` — "Output path '{0}' must be a file path, not a directory."
- `PlanErrorOutputNotWritable` — "Output path '{0}' is not writable: {1}"

**Apply command errors:**
- `ApplyErrorMissingArgs` — "Error: Missing required arguments for 'apply'. Expected --plan <path> and --out <path>."
- `ApplyErrorPlanNotFound` — "Plan path '{0}' does not exist."
- `ApplyErrorPlanNotReadable` — "Plan path '{0}' is not readable: {1}"
- `ApplyErrorOutputIsDirectory` — "Output path '{0}' must be a file path, not a directory."
- `ApplyErrorOutputNotWritable` — "Output path '{0}' is not writable: {1}"

### 3. Update XAML files to use `{x:Static}` bindings

For each XAML view file, add the strings XML namespace declaration at the top:
```xml
xmlns:strings="clr-namespace:Renamer.UI.Resources.Strings"
```

Replace each hardcoded `Text="..."` and `Placeholder="..."` attribute with:
```xml
Text="{x:Static strings:AppStrings.KeyName}"
```

Affected files:
- `src/Renamer.UI/Views/MainPage.xaml`
- `src/Renamer.UI/Views/WorkflowRailView.xaml`
- `src/Renamer.UI/Views/GenerateWorkspaceView.xaml`
- `src/Renamer.UI/Views/PreviewWorkspaceView.xaml`
- `src/Renamer.UI/Views/ApplyWorkspaceView.xaml`

For format strings used in XAML (e.g. `"{0} total"`), if the binding is a simple one-argument format in a `Label` backed by a converter or `StringFormat`, use `StringFormat` on the binding rather than `{x:Static}` for the format pattern, or extract to a value converter as appropriate to the existing pattern in the codebase.

### 4. Update `PlanViewModel.cs` to use `AppStrings`

Replace all string literals used for:
- Default field values (lines ~32–52)
- Workflow step initialisation (lines ~74–85)
- Generation status/error messages
- Preview status/error messages
- Apply status/error messages

Use `AppStrings.KeyName` directly. For dynamic messages with embedded values, use `string.Format(AppStrings.KeyName, value)`.

### 5. Update `CliCommandHandler.cs` to use `CliStrings`

Replace the static help text array and all error output strings with references to `CliStrings.KeyName`.

### 6. Update the index

`docs/specs/000-index.md` already has an entry for this slice (added when the spec was written). No action required unless the status changes.

## Commands to run

1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Git/PR workflow

0. Setup - change to `main` branch and pull latest from origin to prepare for new work.
1. Branch from current `main` using prefix `codex/` — `codex/290-string-resources`.
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. If a matching GitHub issue exists, ensure it is `In Progress` before opening the PR.
5. Open PR back into `main` with slice ID in title/body.
6. Link the corresponding issue in the PR body:
   - use `Closes #<issue-number>` when the PR completes the slice
   - use `Refs #<issue-number>` only for partial work
7. Include the slice ID in PR title and body.

## Acceptance checks

- `dotnet build Renamer.sln` produces zero warnings and zero errors.
- The UI application launches and all five workspaces display correctly — no blank labels or missing text.
- The CLI `help` command output is identical to before this change.
- The CLI `plan` and `apply` commands produce identical error messages to before this change.
- No hardcoded user-facing string literals remain in any XAML file.
- No hardcoded user-facing string literals remain in `PlanViewModel.cs`.
- No hardcoded user-facing string literals remain in `CliCommandHandler.cs`.
- Structured logging templates in `.Core`, `.Cli`, and `.UI` remain unchanged in source code (not extracted to resources).
- Internal exception message strings (e.g. in `PlanSerializer.cs`, `PlanBuilder.cs`, `RootPathOpener.cs`, `CliLogPathProvider.cs`) remain in source code.
- `AppStrings.resx` and `CliStrings.resx` each contain only the strings listed in this spec and no others.

## Tests

- No new unit tests are required for this slice — it is a mechanical string extraction with no behaviour change.
- The following existing test files assert raw user-facing string literals that must be updated to reference the generated resource accessor instead of inline strings:
  - `src/Renamer.Tests/UI/ApplyFlowViewModelTests.cs` — asserts `ApplyStatusMessage`, `ApplyErrorTitle`, `ApplyErrorMessage`, and `StatusMessage` values directly (e.g. `Assert.Equal("Apply completed with 1 success, 0 skipped, 0 failed.", ...)`)
  - `src/Renamer.Tests/UI/PlanGenerationViewModelTests.cs` — asserts `StatusMessage`, `GenerationStatusMessage`, `GenerationErrorTitle`, and `GenerationErrorMessage` values directly
- Each such assertion must be updated to use the resource key, e.g.:
  ```csharp
  Assert.Equal(AppStrings.ApplyStatusSuccess.Replace("{0}", "1").Replace("{1}", "0").Replace("{2}", "0"), viewModel.ApplyStatusMessage);
  // or more idiomatically:
  Assert.Equal(string.Format(AppStrings.ApplyStatusSuccess, 1, 0, 0), viewModel.ApplyStatusMessage);
  ```
- Add `using Renamer.UI.Resources.Strings;` to any test file that references `AppStrings`.

## Test scope

- All pre-existing tests pass: `dotnet test Renamer.sln` exits 0.
- No test may contain a raw string literal that duplicates a value defined in `AppStrings.resx` or `CliStrings.resx`.

## Expected outputs

- `src/Renamer.UI/Resources/Strings/AppStrings.resx` — new file
- `src/Renamer.UI/Resources/Strings/AppStrings.Designer.cs` — auto-generated by ResXFileCodeGenerator (do not hand-edit)
- `src/Renamer.Cli/Resources/CliStrings.resx` — new file
- `src/Renamer.Cli/Resources/CliStrings.Designer.cs` — auto-generated by ResXFileCodeGenerator (do not hand-edit)
- All five XAML view files updated — `Text`/`Placeholder` attributes replaced with `{x:Static}` bindings
- `src/Renamer.UI/Plans/PlanViewModel.cs` updated — string literals replaced with resource accessors
- `src/Renamer.Cli/Commands/CliCommandHandler.cs` updated — string literals replaced with resource accessors
- `docs/specs/000-index.md` updated

## Future i18n path

Once this slice is merged, adding a second language requires only:
1. Create `AppStrings.fr.resx` (or the appropriate culture code) alongside `AppStrings.resx`.
2. Set the current UI culture at startup: `CultureInfo.CurrentUICulture = new CultureInfo("fr")`.
3. The generated `ResourceManager` will automatically resolve the correct language at runtime.

No code changes to ViewModels, XAML, or CLI command handlers are required.

## Exit criteria

- PR merged with passing tests for this slice only.
- PR links the corresponding GitHub issue when one exists.

## Definition of Done

- Acceptance checks in this slice are satisfied.
- Tests listed in this slice pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `codex/` and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
