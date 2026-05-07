/* global React, ReactDOM */
const { useState } = React;

// ────────────────────────────────────────────────
// Shared bits
// ────────────────────────────────────────────────

function WinBar({ title }) {
  return (
    <div className="winbar">
      <div className="dots"><i/><i/><i/></div>
      <div className="label">Renamer · {title}</div>
      <div style={{ fontFamily: 'var(--hand-mono)', fontSize: 11 }}>— □ ✕</div>
    </div>
  );
}

function ThemeBlob() {
  return (
    <div className="theme-blob">
      <span className="dot sun" /> <span className="dot moon" /> <span className="dot auto" />
      <span style={{ marginLeft: 4 }}>theme</span>
    </div>
  );
}

function StepDot({ n, state }) {
  return <span className={`step-dot ${state}`}>{state === 'done' ? '✓' : n}</span>;
}

function Anno({ children, style }) {
  return <div className="anno" style={style}>{children}</div>;
}

const SAMPLE_FOLDER = 'C:\\Dev\\Renamer\\samples';

// ────────────────────────────────────────────────
// Direction A — One page, no sidebar, smart defaults
// ────────────────────────────────────────────────

function DirectionA() {
  return (
    <div className="sheet">
      <WinBar title="Direction A · single scrolling page" />
      <div className="canvas">
        <div className="dir-stamp">A</div>

        {/* compact header */}
        <div className="row between center wrap gap-12">
          <div className="col gap-6">
            <h2 className="h">Renamer</h2>
            <div className="body">Rename a folder of photos by date in three quick steps.</div>
          </div>
          <ThemeBlob />
        </div>

        {/* top stepper replaces left sidebar */}
        <div className="stepper mt-16" style={{ maxWidth: 720 }}>
          <StepDot n={1} state="now" />
          <span className="lbl">Plan</span>
          <span className="step-line" />
          <StepDot n={2} state="next" />
          <span className="lbl">Review</span>
          <span className="step-line" />
          <StepDot n={3} state="next" />
          <span className="lbl">Rename</span>
        </div>

        {/* main card */}
        <div className="box mt-16" style={{ padding: 18 }}>
          <div className="row between center">
            <h3 className="h">1. Pick your photo folder</h3>
            <span className="pill">step 1 of 3</span>
          </div>

          <div className="kv mt-12">
            <span className="lbl">Photos</span>
            <div className="field">
              <span className="grow">{SAMPLE_FOLDER}</span>
            </div>
            <button className="btn sm">Browse…</button>
          </div>

          {/* defaults panel */}
          <div className="box dashed flat mt-12" style={{ padding: 12 }}>
            <div className="row between center">
              <span className="lbl">Plan saved alongside your photos</span>
              <button className="btn sm ghost">Change… ▾</button>
            </div>
            <div className="body mt-4" style={{ color: 'var(--ink-soft)' }}>
              <span style={{ fontFamily: 'var(--hand-mono)', fontSize: 12 }}>
                {SAMPLE_FOLDER}\<b>rename-plan.json</b>
              </span>
            </div>
          </div>

          <div className="row mt-16 gap-10 center">
            <button className="btn primary lg">Build plan →</button>
            <span className="body" style={{ color: 'var(--ink-soft)' }}>
              We'll scan the folder and propose new names.
            </span>
          </div>
        </div>

        <Anno style={{ top: 30, right: 18, width: 200 }}>
          <b>Title shrinks.</b><br/>
          Steps live in a stepper.
          <span className="arrow">↙</span>
        </Anno>

        <Anno style={{ top: 240, right: 18, width: 200 }}>
          Defaults baked in:<br/>
          <b>save = photo folder</b>,<br/>
          <b>filename = rename-plan.json</b>.<br/>
          Revealed only if needed.
          <span className="arrow">↙</span>
        </Anno>

        <Anno style={{ top: 470, right: 18, width: 200 }}>
          One <b>Build plan</b> button.<br/>
          No full-width orange.
          <span className="arrow">↙</span>
        </Anno>
      </div>
    </div>
  );
}

// ────────────────────────────────────────────────
// Direction B — Wizard with thin numbered rail
// ────────────────────────────────────────────────

function DirectionB() {
  return (
    <div className="sheet">
      <WinBar title="Direction B · wizard with rail" />
      <div className="canvas" style={{ padding: 0 }}>
        <div className="dir-stamp">B</div>

        <div className="row" style={{ minHeight: 720 }}>
          {/* Thin rail */}
          <div className="rail">
            <StepDot n={1} state="done" />
            <span className="lbl-tight" style={{ writingMode: 'vertical-rl', transform: 'rotate(180deg)' }}>plan</span>
            <span className="rail-line" />
            <StepDot n={2} state="now" />
            <span className="lbl-tight" style={{ writingMode: 'vertical-rl', transform: 'rotate(180deg)' }}>review</span>
            <span className="rail-line" />
            <StepDot n={3} state="next" />
            <span className="lbl-tight" style={{ writingMode: 'vertical-rl', transform: 'rotate(180deg)' }}>rename</span>
          </div>

          {/* main */}
          <div className="grow col gap-16" style={{ padding: '22px 26px 32px' }}>
            <div className="row between center wrap">
              <div>
                <div className="lbl">Step 2 of 3</div>
                <h2 className="h">Review the plan</h2>
                <div className="body">2 folder changes proposed · 0 problems</div>
              </div>
              <ThemeBlob />
            </div>

            {/* summary strip */}
            <div className="row gap-10 wrap">
              <div className="stat"><div className="num">2</div><div className="lbl">changes</div></div>
              <div className="stat"><div className="num">0</div><div className="lbl">notes</div></div>
              <div className="stat" style={{ background: '#f5f1e6' }}>
                <div className="lbl">from</div>
                <div className="body" style={{ fontFamily: 'var(--hand-mono)', fontSize: 12 }}>{SAMPLE_FOLDER}</div>
              </div>
            </div>

            {/* plan cards */}
            <div className="col gap-8">
              <h4 className="h">Proposed names</h4>
              <div className="plan-card">
                <div className="col">
                  <span className="from">TestPhotos</span>
                  <span className="to"><b>2017-11-08 — 2025-08-17 — TestPhotos</b></span>
                </div>
                <span className="pill note">2 photos · ok</span>
              </div>
              <div className="plan-card">
                <div className="col">
                  <span className="from">Holiday-Crete</span>
                  <span className="to"><b>2024-06-02 — 2024-06-14 — Holiday-Crete</b></span>
                </div>
                <span className="pill note">38 photos · ok</span>
              </div>
            </div>

            {/* footer actions */}
            <div className="row between center mt-12 wrap gap-10">
              <button className="btn ghost sm">← Back to plan</button>
              <div className="row gap-10">
                <button className="btn">Reload plan</button>
                <button className="btn primary lg">Continue →</button>
              </div>
            </div>
          </div>
        </div>

        <Anno style={{ top: 60, right: 18, width: 200 }}>
          Sidebar collapses to a<br/>
          <b>thin numbered rail</b>.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 240, right: 18, width: 200 }}>
          Stat strip replaces the<br/>
          three "Before you rename"<br/>
          cards.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 460, right: 18, width: 200 }}>
          Each folder = one<br/>
          <b>plan card</b> with a<br/>
          status pill.
          <span className="arrow">↙</span>
        </Anno>
      </div>
    </div>
  );
}

// ────────────────────────────────────────────────
// Direction C — Compact, two-column with inline buttons
// ────────────────────────────────────────────────

function DirectionC() {
  return (
    <div className="sheet">
      <WinBar title="Direction C · compact two-column" />
      <div className="canvas">
        <div className="dir-stamp">C</div>

        <div className="row between center wrap gap-12">
          <div className="col">
            <h3 className="h">Renamer</h3>
            <div className="body">Plan · Review · Rename</div>
          </div>
          <div className="row gap-10 center">
            <span className="pill done">✓ plan</span>
            <span className="pill now">review</span>
            <span className="pill next">rename</span>
            <ThemeBlob />
          </div>
        </div>

        {/* Two columns: settings (left) | results preview (right) */}
        <div className="grid-2 mt-16">
          <div className="box">
            <h4 className="h">Plan</h4>
            <div className="body" style={{ marginBottom: 10 }}>Pick a folder, the rest is automatic.</div>

            <div className="kv">
              <span className="lbl">Photos</span>
              <div className="field"><span className="grow">{SAMPLE_FOLDER}</span></div>
              <button className="btn icon">…</button>
            </div>

            <div className="kv">
              <span className="lbl">Save plan</span>
              <div className="field muted"><span className="grow">same folder</span></div>
              <button className="btn icon ghost">change</button>
            </div>

            <div className="kv">
              <span className="lbl">Filename</span>
              <div className="field muted"><span className="grow">rename-plan.json</span></div>
              <button className="btn icon ghost">edit</button>
            </div>

            <div className="row mt-16 between center">
              <span className="body" style={{ color: 'var(--ink-soft)' }}>2 subfolders found</span>
              <button className="btn primary">Build plan</button>
            </div>
          </div>

          <div className="box flat" style={{ background: 'rgba(255,255,255,.35)' }}>
            <div className="row between center">
              <h4 className="h">Preview</h4>
              <span className="lbl">live</span>
            </div>
            <div className="body" style={{ marginBottom: 10 }}>
              What the new names will look like:
            </div>
            <div className="col gap-8">
              <div className="plan-card">
                <div className="col">
                  <span className="from strike">TestPhotos</span>
                  <span className="to">2017-11-08 — 2025-08-17 — TestPhotos</span>
                </div>
                <span className="pill done">ok</span>
              </div>
              <div className="plan-card">
                <div className="col">
                  <span className="from strike">Holiday-Crete</span>
                  <span className="to">2024-06-02 — 2024-06-14 — Holiday-Crete</span>
                </div>
                <span className="pill done">ok</span>
              </div>
              <div className="plan-card" style={{ background: '#fff7e8' }}>
                <div className="col">
                  <span className="from strike">Random</span>
                  <span className="to">— Random</span>
                </div>
                <span className="pill warn">no dates</span>
              </div>
            </div>
          </div>
        </div>

        <Anno style={{ top: 60, right: 18, width: 200 }}>
          <b>Inline icon buttons</b>:<br/>
          field + tiny browse.<br/>
          No giant orange bars.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 280, right: 18, width: 200 }}>
          Plan + filename:<br/>
          <b>muted defaults</b>.<br/>
          One click to override.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 500, right: 18, width: 200 }}>
          Right column =<br/>
          <b>live preview</b>.<br/>
          Review built in.
          <span className="arrow">↙</span>
        </Anno>
      </div>
    </div>
  );
}

// ────────────────────────────────────────────────
// Direction D — Drop-in single screen, zero-config
// ────────────────────────────────────────────────

function DirectionD() {
  return (
    <div className="sheet">
      <WinBar title="Direction D · drop & rename" />
      <div className="canvas">
        <div className="dir-stamp">D</div>

        <div className="row between center wrap gap-12">
          <h3 className="h">Renamer</h3>
          <ThemeBlob />
        </div>

        {/* drop zone */}
        <div className="drop mt-16">
          <h2 className="h">Drop a folder here</h2>
          <div className="body">…or <a className="scribble">browse</a> to pick one.</div>
          <div className="lbl mt-8">we'll suggest new dated names</div>
        </div>

        {/* once a folder is chosen */}
        <div className="box mt-16">
          <div className="row between center wrap gap-10">
            <div className="col">
              <span className="lbl">Selected</span>
              <span style={{ fontFamily: 'var(--hand-mono)', fontSize: 13 }}>{SAMPLE_FOLDER}</span>
            </div>
            <div className="row gap-8">
              <button className="btn ghost sm">Change folder</button>
              <button className="btn ghost sm">Advanced ▾</button>
            </div>
          </div>

          <div className="col gap-8 mt-12">
            <div className="plan-card">
              <div className="col">
                <span className="from strike">TestPhotos</span>
                <span className="to">2017-11-08 — 2025-08-17 — TestPhotos</span>
              </div>
              <input type="checkbox" defaultChecked style={{ width: 20, height: 20 }}/>
            </div>
            <div className="plan-card">
              <div className="col">
                <span className="from strike">Holiday-Crete</span>
                <span className="to">2024-06-02 — 2024-06-14 — Holiday-Crete</span>
              </div>
              <input type="checkbox" defaultChecked style={{ width: 20, height: 20 }}/>
            </div>
            <div className="plan-card" style={{ background: '#fff7e8' }}>
              <div className="col">
                <span className="from strike">Misc</span>
                <span className="to ph-block" style={{ display: 'inline-block' }}>no dates found — skipped</span>
              </div>
              <input type="checkbox" style={{ width: 20, height: 20 }}/>
            </div>
          </div>

          <div className="row between center mt-16 wrap gap-10">
            <span className="body" style={{ color: 'var(--ink-soft)' }}>
              2 of 3 folders selected · plan auto-saved next to photos
            </span>
            <button className="btn primary lg">Rename 2 folders</button>
          </div>
        </div>

        <Anno style={{ top: 60, right: 18, width: 200 }}>
          <b>One screen.</b><br/>
          No steps, no plan file UI,<br/>
          no review tab.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 280, right: 18, width: 200 }}>
          Plan saved silently<br/>
          beside photos.<br/>
          <b>Advanced ▾</b> reveals<br/>
          path overrides.
          <span className="arrow">↙</span>
        </Anno>
        <Anno style={{ top: 520, right: 18, width: 200 }}>
          Per-folder <b>checkboxes</b><br/>
          + one rename button<br/>
          that names its action.
          <span className="arrow">↙</span>
        </Anno>
      </div>
    </div>
  );
}

// ────────────────────────────────────────────────
// Copy comparison + structural notes
// ────────────────────────────────────────────────

function CopyTable() {
  const rows = [
    ['Page title', 'Rename your folders in three steps', 'Renamer'],
    ['Subtitle', "Build a plan, review the changes, then rename the folders when you're ready.", 'Plan · Review · Rename'],
    ['Step 1 title', 'Build a plan', 'Pick a folder'],
    ['Step 1 sub', 'Choose your photo folder and where to save the plan, then review the folder names before anything changes.', '—'],
    ['Section title', 'Choose folders', '—'],
    ['Field 1 label', 'Photo folder', 'Photos'],
    ['Field 2 label', 'Save folder', '(hidden by default — same folder)'],
    ['Field 3 label', 'Plan file', '(hidden by default — rename-plan.json)'],
    ['Helper line', 'Plan will be saved to C:\\Dev\\…\\rename-plan.json', 'Saved next to your photos'],
    ['Big button', 'Choose photo folder', 'Browse…'],
    ['Confirm button', 'Build plan', 'Build plan →'],
    ['Step 2 sub', 'Load a saved plan and check the proposed folder names before anything is renamed.', 'Check the new names look right.'],
    ['"Before you rename"', 'Created · Folder changes · Things to note', '2 changes · 0 notes'],
    ['Step 3 title', 'Apply the plan', 'Rename'],
    ['Step 3 sub', 'This step makes the folder name changes. Review the plan first, then run it when you are ready.', '—'],
    ['Run button', 'Rename now', 'Rename 2 folders'],
    ['Done state', 'Rename finished: 1 changed, 0 skipped, 0 failed.', '✓ Renamed 1 folder'],
  ];
  return (
    <div className="box">
      <h3 className="h">Copy: before → after</h3>
      <div className="body" style={{ marginBottom: 10 }}>
        Friendly but minimal. Each piece of UI says one thing.
      </div>
      <table className="copy-tbl">
        <thead><tr><th>where</th><th>now</th><th>proposed</th></tr></thead>
        <tbody>
          {rows.map((r, i) => (
            <tr key={i}>
              <td className="lbl" style={{ whiteSpace: 'nowrap' }}>{r[0]}</td>
              <td className="old">{r[1]}</td>
              <td className="new"><b>{r[2]}</b></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function StructureNotes() {
  return (
    <div className="footer-notes">
      <div className="note-card">
        <b>Defaults in, fields out</b><br/>
        Save folder defaults to the photo folder. Filename defaults to <code>rename-plan.json</code>. Both hidden behind "Advanced" — surface only on conflict.
      </div>
      <div className="note-card">
        <b>No full-width orange</b><br/>
        Browse becomes a small button beside the field. Only the <i>commit</i> action ("Build plan", "Rename") is colored, and right-aligned.
      </div>
      <div className="note-card">
        <b>Cards over floaty rows</b><br/>
        Each folder change is a self-contained card with from/to and a status pill. No wandering labels above loose values.
      </div>
      <div className="note-card">
        <b>Page title shrinks</b><br/>
        "Rename your folders in three steps" becomes a stepper. The H1 just says <b>Renamer</b> or the current step.
      </div>
      <div className="note-card">
        <b>Sidebar → stepper or rail</b><br/>
        The three-card sidebar takes too much room for what it says. A horizontal stepper or a thin numbered rail does the same job.
      </div>
      <div className="note-card">
        <b>Theme toggle compact</b><br/>
        Three radio buttons collapse to a single 3-dot pill (sun · moon · auto). One click cycles, hover names the mode.
      </div>
    </div>
  );
}

// ────────────────────────────────────────────────
// Top-level app
// ────────────────────────────────────────────────

const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "view": "all",
  "style": "sketch",
  "showAnnos": true
}/*EDITMODE-END*/;

function App() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const [view, setView] = useState(t.view || 'all');

  React.useEffect(() => {
    document.body.classList.toggle('style-clean', t.style === 'clean');
    document.body.classList.toggle('hide-annos', !t.showAnnos);
  }, [t.style, t.showAnnos]);

  // sync local view with tweak
  React.useEffect(() => { setView(t.view || 'all'); }, [t.view]);

  const setView2 = (v) => { setView(v); setTweak('view', v); };

  const tabs = [
    ['all', 'All four'],
    ['A', 'A · One page'],
    ['B', 'B · Wizard rail'],
    ['C', 'C · Two-column'],
    ['D', 'D · Drop & go'],
    ['copy', 'Copy rewrites'],
  ];

  return (
    <div className="page">
      <div className="page-head">
        <div className="col">
          <div className="meta">Wireframes · v1 · low-fi</div>
          <h1 className="page-title">Renamer — cleanup directions</h1>
          <p className="page-sub">
            Four ways to declutter the three-step flow. All directions: shrink the page title,
            kill full-width orange buttons, default the save-folder + filename, and structure plan items as cards.
          </p>
        </div>
        <div className="col gap-8" style={{ alignItems: 'flex-end' }}>
          <div className="legend">
            <span><span className="swatch s1"/>commit action</span>
            <span><span className="swatch s2"/>done</span>
            <span><span className="swatch s3"/>defaults</span>
          </div>
        </div>
      </div>

      <div className="tabs" data-screen-label="00 Tabs">
        {tabs.map(([id, label]) => (
          <button key={id} className={`tab ${view === id ? 'on' : ''}`} onClick={() => setView2(id)}>
            {label}
          </button>
        ))}
      </div>

      {(view === 'all' || view === 'A') && (
        <div data-screen-label="A One page" style={{ marginBottom: 28 }}>
          <div className="dir-head">
            <span className="dir-num">A</span>
            <span className="dir-name">One page, smart defaults</span>
            <div className="dir-tags">
              <span className="pill">no sidebar</span>
              <span className="pill">stepper</span>
              <span className="pill">defaults hidden</span>
            </div>
          </div>
          <div className="dir-desc">
            Drop the sidebar entirely. Top stepper gives you the three steps at a glance.
            Save folder + filename are defaulted and tucked into a "Change…" reveal.
          </div>
          <DirectionA />
        </div>
      )}

      {(view === 'all' || view === 'B') && (
        <div data-screen-label="B Wizard rail" style={{ marginBottom: 28 }}>
          <div className="dir-head">
            <span className="dir-num">B</span>
            <span className="dir-name">Wizard with thin rail</span>
            <div className="dir-tags">
              <span className="pill">numbered rail</span>
              <span className="pill">card list</span>
              <span className="pill">stat strip</span>
            </div>
          </div>
          <div className="dir-desc">
            Sidebar shrinks to a 70px rail of numbered dots — keeps step nav, frees the canvas.
            Stat strip replaces the "Created / Folder changes / Things to note" trio.
            Plan items become cards with status pills.
          </div>
          <DirectionB />
        </div>
      )}

      {(view === 'all' || view === 'C') && (
        <div data-screen-label="C Two-column" style={{ marginBottom: 28 }}>
          <div className="dir-head">
            <span className="dir-num">C</span>
            <span className="dir-name">Compact two-column</span>
            <div className="dir-tags">
              <span className="pill">inline buttons</span>
              <span className="pill">live preview</span>
              <span className="pill">muted defaults</span>
            </div>
          </div>
          <div className="dir-desc">
            Left = the (small) inputs, right = a live preview of proposed names.
            Browse buttons live next to their fields. Plan + filename render as muted "you can edit, but you don't need to" rows.
          </div>
          <DirectionC />
        </div>
      )}

      {(view === 'all' || view === 'D') && (
        <div data-screen-label="D Drop and go" style={{ marginBottom: 28 }}>
          <div className="dir-head">
            <span className="dir-num">D</span>
            <span className="dir-name">Drop & rename</span>
            <div className="dir-tags">
              <span className="pill">zero-config</span>
              <span className="pill">checkboxes</span>
              <span className="pill">advanced ▾</span>
            </div>
          </div>
          <div className="dir-desc">
            The boldest cut. No "plan" surface at all from the user's POV — drop a folder, see proposed names,
            uncheck any you don't want, hit rename. The plan file still exists; it's just an implementation detail.
            "Advanced" hides path overrides for the rare edge case.
          </div>
          <DirectionD />
        </div>
      )}

      {(view === 'all' || view === 'copy') && (
        <div data-screen-label="Copy rewrites" style={{ marginBottom: 28 }}>
          <div className="dir-head">
            <span className="dir-name">Copy rewrites</span>
          </div>
          <CopyTable />
          <h3 className="h" style={{ marginTop: 24 }}>Structural moves (apply to all directions)</h3>
          <StructureNotes />
        </div>
      )}

      <TweaksPanel>
        <TweakSection label="View" />
        <TweakRadio
          label="Direction"
          value={t.view}
          options={[
            { value: 'all', label: 'All' },
            { value: 'A', label: 'A' },
            { value: 'B', label: 'B' },
            { value: 'C', label: 'C' },
            { value: 'D', label: 'D' },
          ]}
          onChange={(v) => setTweak('view', v)}
        />
        <TweakSection label="Style" />
        <TweakRadio
          label="Render"
          value={t.style}
          options={[
            { value: 'sketch', label: 'Sketchy' },
            { value: 'clean',  label: 'Clean' },
          ]}
          onChange={(v) => setTweak('style', v)}
        />
        <TweakToggle
          label="Show annotations"
          value={t.showAnnos}
          onChange={(v) => setTweak('showAnnos', v)}
        />
      </TweaksPanel>

      <style>{`
        body.hide-annos .anno { display: none; }
      `}</style>
    </div>
  );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
