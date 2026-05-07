# 335 UI Cleanup Design Baseline

## Slice ID
`335-ui-cleanup-design-baseline`

## Goal
Capture the visual and structural decisions for the v2 UI cleanup so subsequent implementation slices (340–390) share a single source of truth, and so per-slice specs can stay focused on a single concrete change.

## In scope
- New `docs/specs/335-ui-cleanup-design-baseline.md` describing:
  - Design problems being solved (clutter, instructional copy, full-width primary buttons, floaty plan items, oversized title, defaults users should not have to specify).
  - Selected direction: **thin numbered rail** with vertical stacked-letter step labels, replacing the current full-card rail.
  - Visual primitives the implementation slices must converge on: small-circle step indicator, stacked-letter labels, cards-not-rows for plan items, inline icon-sized browse buttons, muted "advanced" defaults block, compact icon theme toggle.
  - Defaults policy: save folder defaults to the photo folder, plan filename defaults to `rename-plan.json`; both surfaces hidden behind an `Advanced ▾` disclosure unless overridden.
  - Copy voice: friendly but minimal, single-purpose per element.
  - Map of the implementation slices that follow this one (340–390) and what each owns.
- Pointer to the wireframe HTML file used to derive the baseline.
- Cross-references back to `260-maui-stepper-shell.md`, `300-ui-copy-baseline.md`, `310/320/330` copy refresh slices.

## Out of scope
- Any source code changes (`src/**`).
- Any `AppStrings.resx` changes.
- Any test changes.
- Pixel-precise tokens (colors, exact font sizes) — those are decided in the hi-fi mock that lands alongside slice 340 and become enforceable in 340+ specs.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c docs/335-ui-cleanup-design-baseline`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `docs/335-ui-cleanup-design-baseline`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit other files until steps 1–6 are complete.

## Implementation steps
1. Add `docs/specs/335-ui-cleanup-design-baseline.md` with the structure described in "In scope".
2. Add an entry for this slice and the planned 340–390 slices to `docs/checklists/v1.md` under a new "UI cleanup v2 (one PR per item)" section.
3. Cross-link from `docs/specs/000-index.md` (under whichever section currently lists UI specs) to `335` so it appears in the spec index.
4. Do not modify any file outside `docs/`.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

(Tests are unaffected; build/test exists to satisfy the standard command sequence and confirm the docs-only change has not broken the working tree.)

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `docs/`.
2. Keep the PR scoped to documentation changes only — no `src/**` edits.
3. Title the PR with a Conventional Commit, for example `docs(specs): 335 ui cleanup design baseline`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- `docs/specs/335-ui-cleanup-design-baseline.md` exists and lists the 340–390 implementation slices with one-line summaries.
- `docs/checklists/v1.md` has a new "UI cleanup v2" section with this slice and the 340–390 slices listed and unchecked (except 335 itself, ticked at PR merge time).
- `docs/specs/000-index.md` references `335`.
- No files outside `docs/` are modified.

## Tests
- None — this is a documentation-only slice.

## Test scope
- Standard `restore` / `build` / `test` must still pass on `main` after the docs are added.

## Expected outputs
- `docs/specs/335-ui-cleanup-design-baseline.md` (new)
- `docs/checklists/v1.md` (updated — new section + entry)
- `docs/specs/000-index.md` (updated — index entry)

## Exit criteria
- Implementation slices 340–390 can be drafted and reviewed against `335` without re-litigating overall direction.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Standard command sequence passes locally.
- Checklist entry for `335` is updated to checked at merge.
- Branch pre-implementation gate completed before first edit.
- PR scope is limited to `docs/` changes only.

---

# Design baseline content (the document body itself)

> The section below is the **content** that lives inside `335-ui-cleanup-design-baseline.md` once this slice is executed. It is included here so the slice's "expected output" is concrete and reviewable.

## Why we are redesigning

The current desktop UI works but suffers from four cumulative problems:

1. **Wordy and instructional.** Headings such as "Rename your folders in three steps" plus per-step paragraphs of explanation crowd out the actual interface.
2. **Cluttered rail.** The left-side stepper currently uses three large cards with title + description + status badge, taking ~280px of horizontal space per step.
3. **Visual hierarchy is unclear.** Full-width orange primary buttons appear next to every path field, giving equal visual weight to "browse" and "commit", and crowding the canvas.
4. **Overspecification.** Users must pick a save folder and plan filename even though sensible defaults exist (save next to the photos, name the file `rename-plan.json`).

## Selected direction

From four wireframe directions explored, the chosen approach is **Direction B: thin numbered rail with vertical stacked-letter labels**, with the supporting decisions from Directions A, C, and D folded in where they apply to specific surfaces:

- **Rail** — collapses from 280px-wide cards to a ~70px column. Each step is a numbered circular indicator (1 / 2 / 3) with state (done / now / next / error) and a vertical label drawn as a stack of single-letter `Label`s (e.g. `P / L / A / N`).
- **Buttons** — full-width orange primary buttons next to path fields are replaced by small inline icon-sized browse buttons. The accent-colored primary is reserved for the single commit action of each screen (`Build plan`, `Continue`, `Rename N folders`).
- **Defaults** — save folder and plan filename are no longer required user input. The view defaults `OutputDirectoryPath` to the chosen `RootPath`, and `PlanFileName` to `rename-plan.json`. Both fields are hidden behind an `Advanced ▾` disclosure, surfaced only when the user wants to override.
- **Plan items** — the preview list becomes a vertical list of self-contained cards. Each card has from→to in monospace, a status pill on the right, and is bordered as a single bounded surface — no labels floating above loose values.
- **Stat strip** — the "Created / Folder changes / Things to note" trio collapses to a small horizontal stat strip (count + label per stat) sitting directly above the plan-item list.
- **Theme toggle** — three `RadioButton`s collapse to a single icon-pill control that cycles Light → Dark → System on tap and announces the current mode in its tooltip.
- **Page title** — "Rename your folders in three steps" shrinks. The H1 in the header bar just says "Renamer"; the active step name is shown by the workspace itself.

## Voice

Friendly but minimal. Each piece of UI says one thing. Avoid second-person instructional sentences when a label and a button can carry the same meaning. The exact key-by-key copy already lives in `AppStrings.resx` after slices 300/310/320/330 — this baseline does not re-litigate strings, but each implementation slice may add small new keys (e.g. `AdvancedDisclosureLabel`) which must follow the same voice.

## Vertical labels

The rail's per-step labels (`PLAN`, `PREVIEW`, `RENAME`) are rendered as **stacked single-letter `Label`s in a `VerticalStackLayout`** — not via `Rotation="-90"`. This keeps text crisp at small sizes, supports localization (the resource value is split per character at bind time, or pre-split by the ViewModel into a string list), and avoids antialiasing issues seen with rotated MAUI `Label`s on Windows.

Localization note: this works for Latin scripts but assumes one glyph per character. If/when the project ships in a language with combining characters or vertical-by-default scripts, the rail label binding will need to switch from per-char split to a layout strategy chosen by the active culture. That is **out of scope for v1** but called out here so it surfaces in any future locale work.

## Implementation slice map

| Slice | Type | What it owns |
|---|---|---|
| `340` | `refactor(ui)` | Thin numbered rail — replace card-style steps with circular numbered indicator + stacked-letter vertical label. Rail column width drops from 280px to ~70px. |
| `350` | `refactor(ui)` | Inline icon-sized browse buttons. Remove full-width primary buttons next to path fields; primary accent reserved for commit actions. |
| `360` | `feat(ui)` | Smart defaults for plan file. Save folder defaults to root folder, filename defaults to `rename-plan.json`, both behind an `Advanced ▾` disclosure. |
| `370` | `refactor(ui)` | Plan items as bounded cards. Restructure preview list rows into self-contained from→to cards with a status pill. |
| `380` | `refactor(ui)` | Stat strip + condensed apply-result banner. Replace "Before you rename" trio + Apply detail grid with compact summary blocks. |
| `390` | `refactor(ui)` | Compact theme toggle. Three `RadioButton`s collapse to a single icon-pill cycling Light/Dark/System; menu bar item kept for parity. |

Slices are independent enough to land in any order after `340`, but the recommended order matches the table.

## References

- Wireframe artifact (low-fi, four directions explored): `Renamer wireframes.html` in the design workspace.
- Current rail implementation: `src/Renamer.UI/Views/WorkflowRailView.xaml`.
- Current step model: `src/Renamer.UI/Plans/PlanWorkflowStepItem.cs`, `PlanWorkflowStep.cs`, `PlanWorkflowStepStatus.cs`.
- Stepper-shell origin: `docs/specs/260-maui-stepper-shell.md`.
- Copy voice baseline: `docs/specs/300-ui-copy-baseline.md`.
