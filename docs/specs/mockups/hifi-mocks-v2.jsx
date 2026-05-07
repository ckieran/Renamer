/* global React, ReactDOM, DCSection, DCArtboard */
const { useState } = React;

// ─────────────────────────────────────────────
// Step rail
// ─────────────────────────────────────────────

function VLabel({ text }) {
  return (
    <div className="rn-vlabel">
      {text.split('').map((c, i) => <span key={i}>{c}</span>)}
    </div>
  );
}

function Rail({ active }) {
  // active: 'plan' | 'review' | 'rename'
  const order = ['plan', 'review', 'rename'];
  const labels = { plan: 'PLAN', review: 'REVIEW', rename: 'RENAME' };
  const numbers = { plan: 1, review: 2, rename: 3 };
  const idx = order.indexOf(active);
  return (
    <div className="rn-rail">
      {order.map((key, i) => {
        const isNow = i === idx;
        const isDone = i < idx;
        const stateClass = isNow ? 'now' : isDone ? 'done' : 'next';
        return (
          <React.Fragment key={key}>
            {i > 0 && <div className="rn-rail-line" />}
            <div className={`rn-step is-${stateClass}`}>
              <div className={`rn-dot ${stateClass}`}>
                {isDone ? '✓' : numbers[key]}
              </div>
              <VLabel text={labels[key]} />
            </div>
          </React.Fragment>
        );
      })}
    </div>
  );
}

// ─────────────────────────────────────────────
// Window chrome
// ─────────────────────────────────────────────

function Window({ theme, children }) {
  return (
    <div className={`rn-window rn-${theme}`}>
      <div className="rn-titlebar">
        <div className="menu">
          <span>File</span><span>View</span><span>Help</span>
        </div>
        <div>Renamer</div>
        <div>— □ ✕</div>
      </div>
      {children}
    </div>
  );
}

function Header({ title, sub }) {
  return (
    <div className="rn-header">
      <div>
        <h1 className="rn-title">{title}</h1>
        {sub && <p className="rn-sub">{sub}</p>}
      </div>
      <div className="rn-theme-pill">
        <span className="glyph" />
        <span>Theme</span>
      </div>
    </div>
  );
}

// ─────────────────────────────────────────────
// PLAN screen
// ─────────────────────────────────────────────

function PlanScreen({ theme, advanced }) {
  return (
    <Window theme={theme}>
      <div className="rn-page">
        <Header title="Renamer" sub="Plan · Review · Rename" />
        <div className="rn-main">
          <Rail active="plan" />
          <div className="rn-workspace">
            <div className="rn-ws-head">
              <div className="crumbs">Step 1 of 3</div>
              <h2>Pick a folder</h2>
              <p>Choose the folder of photos you'd like to rename.</p>
            </div>

            <div className="rn-field-row">
              <div className="rn-label">Photos</div>
              <div className="rn-input">C:\Dev\Renamer\samples</div>
              <div className="rn-icon-btn">📁</div>
            </div>

            <div className="rn-advanced">
              <div className="rn-disc-row">
                <div className="hint">Plan saved beside your photos as <code>rename-plan.json</code></div>
                <button className="rn-disc-btn">{advanced ? 'Hide advanced ▴' : 'Advanced ▾'}</button>
              </div>

              {advanced && (
                <div className="rn-advanced-body">
                  <div className="rn-field-row">
                    <div className="rn-label">Save folder</div>
                    <div className="rn-input">C:\Dev\Renamer\samples</div>
                    <div className="rn-icon-btn">📁</div>
                  </div>
                  <div className="rn-field-row">
                    <div className="rn-label">Filename</div>
                    <div className="rn-input">rename-plan.json</div>
                    <div className="rn-icon-btn">✎</div>
                  </div>
                </div>
              )}
            </div>

            <div style={{ flex: 1 }} />

            <div className="rn-actions">
              <span className="meta">2 subfolders found</span>
              <div className="group">
                <button className="rn-btn">Build plan</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Window>
  );
}

// ─────────────────────────────────────────────
// REVIEW screen
// ─────────────────────────────────────────────

const PLAN_ITEMS = [
  { from: 'TestPhotos', to: '2017-11-08 — 2025-08-17 — TestPhotos', status: 'ok', label: 'ok · 2 photos' },
  { from: 'Holiday-Crete', to: '2024-06-02 — 2024-06-14 — Holiday-Crete', status: 'ok', label: 'ok · 38 photos' },
  { from: 'Birthday-2023', to: '2023-04-12 — 2023-04-12 — Birthday-2023', status: 'ok', label: 'ok · 14 photos' },
  { from: 'Misc-Random', to: 'Misc-Random', status: 'warn', label: 'no dates' },
];

function ReviewScreen({ theme }) {
  return (
    <Window theme={theme}>
      <div className="rn-page">
        <Header title="Renamer" sub="Plan · Review · Rename" />
        <div className="rn-main">
          <Rail active="review" />
          <div className="rn-workspace">
            <div className="rn-ws-head">
              <div className="crumbs">Step 2 of 3</div>
              <h2>Review the plan</h2>
              <p>Check the new names look right before anything is renamed.</p>
            </div>

            <div className="rn-stats">
              <div className="rn-stat"><div className="num">3</div><div className="lbl">changes</div></div>
              <div className="rn-stat"><div className="num">1</div><div className="lbl">notes</div></div>
              <div className="rn-stat"><div className="num">54</div><div className="lbl">photos</div></div>
            </div>

            <div className="rn-cards">
              {PLAN_ITEMS.map((it, i) => (
                <div key={i} className={`rn-card ${it.status === 'warn' ? 'warn' : ''}`}>
                  <div>
                    <div className="from">{it.from}</div>
                    <div className="to">{it.to}</div>
                  </div>
                  <span className={`rn-pill ${it.status}`}>{it.label}</span>
                </div>
              ))}
            </div>

            <div className="rn-actions">
              <button className="rn-btn ghost sm">← Back to plan</button>
              <div className="group">
                <button className="rn-btn ghost sm">Reload plan</button>
                <button className="rn-btn">Continue →</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Window>
  );
}

// ─────────────────────────────────────────────
// RENAME (apply finished) screen
// ─────────────────────────────────────────────

function RenameScreen({ theme }) {
  return (
    <Window theme={theme}>
      <div className="rn-page">
        <Header title="Renamer" sub="Plan · Review · Rename" />
        <div className="rn-main">
          <Rail active="rename" />
          <div className="rn-workspace">
            <div className="rn-ws-head">
              <div className="crumbs">Step 3 of 3</div>
              <h2>Rename folders</h2>
              <p>This is the live rename. Review the plan before running it.</p>
            </div>

            <div className="rn-banner">
              <div className="hl"><span className="check">✓</span> Renamed 3 folders</div>
              <div className="breakdown">0 skipped · 0 failed · finished in 0.4s</div>
              <button className="details-toggle">Details ▾</button>
            </div>

            <div className="rn-cards">
              {PLAN_ITEMS.slice(0, 3).map((it, i) => (
                <div key={i} className="rn-card">
                  <div>
                    <div className="from">{it.from}</div>
                    <div className="to">{it.to}</div>
                  </div>
                  <span className="rn-pill ok">renamed</span>
                </div>
              ))}
            </div>

            <div className="rn-actions">
              <span className="meta">Report saved to logs folder</span>
              <button className="rn-btn ghost sm">Open folder</button>
            </div>
          </div>
        </div>
      </div>
    </Window>
  );
}

// ─────────────────────────────────────────────
// Token swatches artboard
// ─────────────────────────────────────────────

function Swatches({ theme }) {
  const tokens = theme === 'light' ? [
    ['BackgroundPrimary', '#F8FAFC'],
    ['BackgroundSecondary', '#FFFFFF'],
    ['BackgroundTertiary', '#F0F4F8'],
    ['AccentMid (button)', '#D97706'],
    ['AccentPrimary', '#854F0B'],
    ['TextPrimary', '#1E293B'],
    ['TextSecondary', '#475569'],
    ['BorderPrimary', '#CBD5E1'],
    ['StatusSuccess', '#166534'],
    ['StatusError', '#9A3412'],
    ['BadgeBackground', '#E2E8F0'],
    ['TextLink', '#0F4C81'],
  ] : [
    ['BackgroundPrimary', '#0D1117'],
    ['BackgroundSecondary', '#161B25'],
    ['BackgroundTertiary', '#1E2535'],
    ['AccentMid', '#EF9F27'],
    ['AccentPrimary (button)', '#854F0B'],
    ['TextPrimary', '#E8ECF4'],
    ['TextSecondary', '#A8B4CC'],
    ['BorderPrimary', '#2A3347'],
    ['StatusSuccess', '#166534'],
    ['StatusError', '#9A3412'],
    ['BadgeBackground', '#1E2535'],
    ['TextLink', '#FAC775'],
  ];
  return (
    <div className="swatches" style={{ background: theme === 'dark' ? '#0D1117' : '#fff', color: theme === 'dark' ? '#E8ECF4' : '#111' }}>
      {tokens.map(([name, hex]) => (
        <div key={name} className="swatch" style={{ background: theme === 'dark' ? '#161B25' : '#fff', borderColor: theme === 'dark' ? '#2A3347' : '#e5e7eb' }}>
          <div className="chip" style={{ background: hex }} />
          <div className="meta">
            <div className="name" style={{ color: theme === 'dark' ? '#E8ECF4' : '#111' }}>{name}</div>
            <div className="hex">{hex}</div>
          </div>
        </div>
      ))}
    </div>
  );
}

// ─────────────────────────────────────────────
// App
// ─────────────────────────────────────────────

function App() {
  return (
    <DesignCanvas title="Renamer · Hi-fi mocks (slice 340)">
      <DCSection id="light" title="Light theme — full flow">
        <DCArtboard id="plan-light" label="01 · Plan (defaults collapsed)" width={1100} height={720}>
          <PlanScreen theme="light" advanced={false} />
        </DCArtboard>
        <DCArtboard id="plan-light-adv" label="01b · Plan (Advanced expanded)" width={1100} height={720}>
          <PlanScreen theme="light" advanced={true} />
        </DCArtboard>
        <DCArtboard id="review-light" label="02 · Review" width={1100} height={720}>
          <ReviewScreen theme="light" />
        </DCArtboard>
        <DCArtboard id="rename-light" label="03 · Rename (finished)" width={1100} height={720}>
          <RenameScreen theme="light" />
        </DCArtboard>
      </DCSection>

      <DCSection id="dark" title="Dark theme — full flow">
        <DCArtboard id="plan-dark" label="01 · Plan" width={1100} height={720}>
          <PlanScreen theme="dark" advanced={false} />
        </DCArtboard>
        <DCArtboard id="review-dark" label="02 · Review" width={1100} height={720}>
          <ReviewScreen theme="dark" />
        </DCArtboard>
        <DCArtboard id="rename-dark" label="03 · Rename" width={1100} height={720}>
          <RenameScreen theme="dark" />
        </DCArtboard>
      </DCSection>

      <DCSection id="tokens" title="Theme tokens (mirrored from LightTheme.xaml / DarkTheme.xaml)">
        <DCArtboard id="tokens-light" label="Light tokens" width={760} height={520}>
          <Swatches theme="light" />
        </DCArtboard>
        <DCArtboard id="tokens-dark" label="Dark tokens" width={760} height={520}>
          <Swatches theme="dark" />
        </DCArtboard>
      </DCSection>
    </DesignCanvas>
  );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
