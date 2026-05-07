# 390 Compact Theme Toggle

## Slice ID
`390-compact-theme-toggle`

## Goal
Replace the three-`RadioButton` Light/Dark/System group in the page header with a single compact icon-pill control that cycles through the three modes on tap and announces the current mode to assistive tech.

## In scope
- Replace the `Border` + `HorizontalStackLayout` of three `RadioButton`s in `MainPage.xaml` with a single tappable control:
  - Visual: a small pill (~80–100px wide) with a sun, moon, or auto glyph depending on current mode, plus the mode name label.
  - Tap cycles Light → Dark → System → Light.
  - Tooltip / `AutomationProperties.Name` reads the current mode in the form `Theme: <mode>`.
- Keep the `View` `MenuFlyoutItem` entries (`OnLightThemeClicked` / `OnDarkThemeClicked` / `OnSystemThemeClicked`) as-is for menu-bar parity.
- Update `ThemeService` to expose:
  - A `CurrentTheme` property (existing or new).
  - A `CycleTheme()` method that advances Light → Dark → System.
- Update `MainPage.xaml.cs` to wire the new control's tap to `ThemeService.CycleTheme()` and to update the visual on `CurrentTheme` changes.
- Add resource keys to `AppStrings.resx`:
  - `ThemeToggleLight` — `Light`
  - `ThemeToggleDark` — `Dark`
  - `ThemeToggleSystem` — `System`
  - `ThemeToggleAccessibilityFormat` — `Theme: {0}` (used for `AutomationProperties.Name`).

## Out of scope
- Removing the `View` menu bar entries.
- Theme color token changes.
- Persisting theme across launches (assume already handled).

## Pre-implementation gate (must pass before code edits)
1. Working tree clean.
2. On `main`.
3. `git pull --ff-only origin main`.
4. `git switch -c refactor/390-compact-theme-toggle`.
5. Confirm branch.
6. Move matching issue to `In Progress`.
7. No code edits until steps 1–6 complete.

## Implementation steps
1. Add `CycleTheme()` to `ThemeService` (and any required `CurrentTheme` getter / event). Cover with unit tests.
2. Replace the three-`RadioButton` block in `MainPage.xaml` with a tappable `Border` containing a small icon glyph and label, bound to `ThemeService.CurrentTheme`.
3. Wire `TapGestureRecognizer` (or `Button` styled as a pill) to `CycleTheme()`. Refresh the visual on theme change.
4. Bind `AutomationProperties.Name` and `ToolTipProperties.Text` via `ThemeToggleAccessibilityFormat`.
5. Remove no-longer-used `OnThemeRadioChanged` handler if redundant; keep it if menu-bar parity still needs it.
6. Add tests for `ThemeService.CycleTheme()` ordering.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~Theme"`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch with prefix `refactor/`.
2. PR scoped to header theme control + service cycling logic + tests.
3. PR title: `refactor(ui): 390 compact theme toggle`.
4. Link issue with `Closes #<issue-number>`.

## Acceptance checks
- The page header shows a single compact theme control, not three radio buttons.
- Tapping cycles Light → Dark → System → Light and the visual updates immediately.
- Assistive tech announces the current mode via `AutomationProperties.Name`.
- The `View` menu bar still exposes the three explicit theme choices.
- All new copy is sourced from `AppStrings.resx`.

## Tests
- `src/Renamer.Tests/UI/ThemeServiceTests.cs` (new or extended) covering `CycleTheme()` ordering across all three modes.

## Expected outputs
- `src/Renamer.UI/Services/ThemeService.cs` — `CycleTheme()` + `CurrentTheme`.
- `src/Renamer.UI/MainPage.xaml` — compact theme control replaces the radio group.
- `src/Renamer.UI/MainPage.xaml.cs` — wiring updated.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) — new keys.
- `src/Renamer.Tests/UI/ThemeServiceTests.cs` — tests.

## Definition of Done
- Acceptance checks satisfied.
- Tests pass.
- Checklist updated.
- Pre-implementation gate completed.
- PR scope limited to this slice.
