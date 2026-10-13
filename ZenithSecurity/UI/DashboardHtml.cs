namespace ZenithSecurity.UI
{
    internal static class DashboardHtml
    {
        public const string Content = """
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8"/>
<meta name="viewport" content="width=device-width,initial-scale=1"/>
<title>Zenith Security Dashboard</title>
<style>
:root{
  --bg:#0f1523;--surface:#131c2e;--raised:#1a2540;--border:#1e2d47;
  --text:#e2e8f0;--muted:#64748b;--accent:#3b82f6;--accent2:#60a5fa;
  --critical:#ef4444;--high:#f97316;--medium:#eab308;--low:#22c55e;--info:#64748b;
}
*{box-sizing:border-box;margin:0;padding:0}
body{background:var(--bg);color:var(--text);font-family:'Segoe UI',system-ui,sans-serif;
     font-size:14px;display:flex;height:100vh;overflow:hidden}

/* ── Sidebar ──────────────────────────────────────────────────────────── */
#sidebar{width:220px;background:var(--surface);border-right:1px solid var(--border);
         display:flex;flex-direction:column;flex-shrink:0}
.brand{padding:18px 16px 12px;border-bottom:1px solid var(--border)}
.brand-name{font-size:13px;font-weight:700;color:var(--text);letter-spacing:.3px}
.brand-sub{font-size:10px;color:var(--muted);margin-top:2px}
.nav-section{padding:10px 0}
.nav-label{font-size:10px;color:var(--muted);padding:6px 16px 4px;
           text-transform:uppercase;letter-spacing:.8px;font-weight:600}
.nav-item{display:flex;align-items:center;gap:10px;padding:9px 16px;
          cursor:pointer;color:var(--muted);font-size:13px;
          border-left:3px solid transparent;transition:all .15s}
.nav-item:hover{color:var(--text);background:var(--raised)}
.nav-item.active{color:var(--accent2);background:var(--raised);
                 border-left-color:var(--accent)}
.nav-icon{font-size:15px;width:18px;text-align:center}
.nav-footer{margin-top:auto;padding:12px 16px;border-top:1px solid var(--border);
            font-size:11px;color:var(--muted)}

/* ── Main ─────────────────────────────────────────────────────────────── */
#main{flex:1;display:flex;flex-direction:column;overflow:hidden}
.view{display:none;flex:1;overflow:auto;padding:20px}
.view.active{display:flex;flex-direction:column}

/* ── Section header ───────────────────────────────────────────────────── */
.sec-header{display:flex;align-items:center;justify-content:space-between;
            margin-bottom:16px;padding-bottom:12px;border-bottom:1px solid var(--border)}
.sec-title{font-size:16px;font-weight:600;color:var(--text)}
.sec-sub{font-size:11px;color:var(--muted);margin-top:2px}

/* ── Cards ────────────────────────────────────────────────────────────── */
.card-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(200px,1fr));gap:12px;margin-bottom:16px}
.card{background:var(--surface);border:1px solid var(--border);border-radius:8px;padding:16px}
.card-title{font-size:11px;color:var(--muted);text-transform:uppercase;letter-spacing:.5px;margin-bottom:6px}
.card-value{font-size:22px;font-weight:700;color:var(--text)}
.card-sub{font-size:11px;color:var(--muted);margin-top:3px}
.card-ok{border-left:3px solid var(--low)}
.card-warn{border-left:3px solid var(--medium)}
.card-crit{border-left:3px solid var(--critical)}
.card-info{border-left:3px solid var(--accent)}

/* ── Status badges ────────────────────────────────────────────────────── */
.badge{display:inline-block;padding:2px 8px;border-radius:3px;font-size:11px;font-weight:600}
.badge-ok{background:rgba(34,197,94,.15);color:var(--low)}
.badge-warn{background:rgba(234,179,8,.15);color:var(--medium)}
.badge-crit{background:rgba(239,68,68,.15);color:var(--critical)}
.badge-info{background:rgba(59,130,246,.15);color:var(--accent2)}
.badge-high{background:rgba(249,115,22,.15);color:var(--high)}

/* ── Tab bars ─────────────────────────────────────────────────────────── */
.tab-bar{display:flex;gap:2px;background:var(--raised);border-radius:6px;
         padding:3px;margin-bottom:16px;width:fit-content}
.tab-btn{padding:7px 14px;border-radius:4px;border:none;background:transparent;
         color:var(--muted);font-size:12px;cursor:pointer;font-family:inherit;
         transition:all .15s;white-space:nowrap}
.tab-btn:hover{color:var(--text)}
.tab-btn.active{background:var(--surface);color:var(--text);font-weight:600;
                box-shadow:0 1px 3px rgba(0,0,0,.3)}
.tab-pane{display:none}.tab-pane.active{display:block}

/* ── Tables ───────────────────────────────────────────────────────────── */
.tbl{width:100%;border-collapse:collapse;font-size:12px}
.tbl th{background:var(--raised);color:var(--muted);font-weight:600;
        text-align:left;padding:8px 12px;border-bottom:1px solid var(--border);
        text-transform:uppercase;letter-spacing:.4px;font-size:10px}
.tbl td{padding:8px 12px;border-bottom:1px solid var(--border);color:var(--text)}
.tbl tr:hover td{background:var(--raised)}

/* ── Buttons ──────────────────────────────────────────────────────────── */
.btn{padding:7px 14px;border-radius:5px;border:none;cursor:pointer;
     font-size:12px;font-family:inherit;transition:all .15s}
.btn-primary{background:var(--accent);color:#fff}
.btn-primary:hover{background:var(--accent2)}
.btn-secondary{background:var(--raised);color:var(--text);border:1px solid var(--border)}
.btn-secondary:hover{border-color:var(--accent)}
.btn-sm{padding:4px 10px;font-size:11px}
.btn-ok{background:rgba(34,197,94,.15);color:var(--low);border:1px solid rgba(34,197,94,.3)}
.btn-danger{background:rgba(239,68,68,.15);color:var(--critical);border:1px solid rgba(239,68,68,.3)}

/* ── Forms ────────────────────────────────────────────────────────────── */
.field{margin-bottom:12px}
.field label{display:block;font-size:11px;color:var(--muted);
             text-transform:uppercase;letter-spacing:.5px;margin-bottom:5px}
.input{background:var(--raised);border:1px solid var(--border);color:var(--text);
       border-radius:5px;padding:8px 12px;font-size:13px;font-family:inherit;width:100%}
.input:focus{outline:none;border-color:var(--accent)}
.input-row{display:flex;gap:8px;align-items:flex-end}
.input-row .input{flex:1}

/* ── Security tool result box ─────────────────────────────────────────── */
.tool-result{background:var(--raised);border:1px solid var(--border);border-radius:6px;
             padding:14px;font-family:'Consolas','Courier New',monospace;
             font-size:12px;white-space:pre-wrap;word-break:break-all;
             min-height:60px;color:var(--muted);margin-top:10px;max-height:300px;overflow:auto}
.tool-result.ok{border-color:rgba(34,197,94,.4);color:var(--text)}
.tool-result.error{border-color:rgba(239,68,68,.4);color:var(--critical)}
.tool-result.running{border-color:rgba(59,130,246,.4);color:var(--accent2)}

/* ── Misc ─────────────────────────────────────────────────────────────── */
.row{display:flex;gap:12px;margin-bottom:12px}
.row>.card{flex:1}
.info-row{display:flex;gap:12px;padding:8px 0;border-bottom:1px solid var(--border)}
.info-label{color:var(--muted);font-size:12px;min-width:140px;flex-shrink:0}
.info-val{color:var(--text);font-size:12px;word-break:break-all}
.spinner{display:inline-block;width:14px;height:14px;border:2px solid var(--border);
         border-top-color:var(--accent);border-radius:50%;animation:spin .8s linear infinite}
@keyframes spin{to{transform:rotate(360deg)}}
.alert{padding:10px 14px;border-radius:5px;font-size:12px;margin-bottom:10px}
.alert-crit{background:rgba(239,68,68,.1);border:1px solid rgba(239,68,68,.3);color:var(--critical)}
.alert-warn{background:rgba(234,179,8,.1);border:1px solid rgba(234,179,8,.3);color:var(--medium)}
.alert-ok{background:rgba(34,197,94,.1);border:1px solid rgba(34,197,94,.3);color:var(--low)}
.alert-info{background:rgba(59,130,246,.1);border:1px solid rgba(59,130,246,.3);color:var(--accent2)}
.pw-bar-wrap{background:var(--raised);border-radius:3px;height:8px;overflow:hidden;margin:6px 0}
.pw-bar{height:100%;border-radius:3px;transition:width .4s,background .4s}
.copy-btn{background:var(--raised);border:1px solid var(--border);color:var(--muted);
          border-radius:3px;padding:2px 8px;cursor:pointer;font-size:10px}
.copy-btn:hover{color:var(--accent2)}
.rec-item{background:var(--surface);border:1px solid var(--border);border-radius:6px;
          padding:14px;margin-bottom:10px}
.rec-item.CRITICAL{border-left:4px solid var(--critical)}
.rec-item.HIGH{border-left:4px solid var(--high)}
.rec-item.MEDIUM{border-left:4px solid var(--medium)}
.rec-item.LOW{border-left:4px solid var(--low)}
.rec-item.INFO{border-left:4px solid var(--accent)}
</style>
</head>
<body>

<!-- SIDEBAR -->
<nav id="sidebar">
  <div class="brand">
    <div class="brand-name">🛡 Zenith Security</div>
    <div class="brand-sub" id="nav-time">Loading…</div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Overview</div>
    <div class="nav-item active" data-view="dashboard"><span class="nav-icon">⊞</span>Dashboard</div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Security</div>
    <div class="nav-item" data-view="defender"><span class="nav-icon">🛡</span>Defender</div>
    <div class="nav-item" data-view="events"><span class="nav-icon">📋</span>Event Logs</div>
    <div class="nav-item" data-view="recommendations"><span class="nav-icon">💡</span>Recommendations</div>
  </div>

  <div class="nav-section">
    <div class="nav-label">System</div>
    <div class="nav-item" data-view="monitoring"><span class="nav-icon">📊</span>Monitoring</div>
    <div class="nav-item" data-view="services"><span class="nav-icon">⚙</span>Services</div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Network</div>
    <div class="nav-item" data-view="network"><span class="nav-icon">🌐</span>Network</div>
    <div class="nav-item" data-view="firewall"><span class="nav-icon">🔥</span>Firewall</div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Tools</div>
    <div class="nav-item" data-view="sectools"><span class="nav-icon">🔒</span>Security Tools</div>
  </div>

  <div class="nav-footer" id="nav-status">Initializing…</div>
</nav>

<!-- MAIN CONTENT -->
<main id="main">

<!-- ── Dashboard ───────────────────────────────────────────────────── -->
<section class="view active" id="view-dashboard">
  <div class="sec-header">
    <div><div class="sec-title">Security Dashboard</div>
         <div class="sec-sub" id="dash-updated">Loading system info…</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadDashboard()">↺ Refresh</button>
  </div>

  <div class="card-grid" id="dash-cards">
    <div class="card card-info"><div class="card-title">Status</div>
      <div class="card-value" id="dc-status">…</div></div>
    <div class="card card-info"><div class="card-title">CPU</div>
      <div class="card-value" id="dc-cpu">…</div><div class="card-sub">Processor load</div></div>
    <div class="card card-info"><div class="card-title">RAM</div>
      <div class="card-value" id="dc-ram">…</div><div class="card-sub" id="dc-ram-sub">…</div></div>
    <div class="card card-info"><div class="card-title">Processes</div>
      <div class="card-value" id="dc-procs">…</div></div>
  </div>

  <div class="row">
    <div class="card" style="flex:1">
      <div class="card-title">System Info</div>
      <div id="dash-sysinfo" style="margin-top:8px">Loading…</div>
    </div>
    <div class="card" style="flex:1">
      <div class="card-title">Defender Status</div>
      <div id="dash-defender" style="margin-top:8px">Loading…</div>
    </div>
  </div>

  <div class="card">
    <div class="card-title" style="margin-bottom:10px">Storage</div>
    <div id="dash-drives">Loading…</div>
  </div>
</section>

<!-- ── Defender ─────────────────────────────────────────────────────── -->
<section class="view" id="view-defender">
  <div class="sec-header">
    <div><div class="sec-title">Windows Defender</div>
         <div class="sec-sub">Antivirus &amp; threat protection</div></div>
    <div style="display:flex;gap:8px">
      <button class="btn btn-secondary btn-sm" onclick="loadDefender()">↺ Refresh</button>
      <button class="btn btn-primary btn-sm" id="scan-btn" onclick="startScan()">▶ Quick Scan</button>
    </div>
  </div>
  <div id="defender-content">Loading…</div>
</section>

<!-- ── Event Logs ────────────────────────────────────────────────────── -->
<section class="view" id="view-events">
  <div class="sec-header">
    <div><div class="sec-title">Event Logs</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadEvents()">↺ Refresh</button>
  </div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="ev-security">Security</button>
    <button class="tab-btn" data-tab="ev-system">System</button>
  </div>
  <div id="tab-ev-security" class="tab-pane active">
    <div id="ev-security-content">Loading…</div></div>
  <div id="tab-ev-system" class="tab-pane">
    <div id="ev-system-content">Loading…</div></div>
</section>

<!-- ── Recommendations ──────────────────────────────────────────────── -->
<section class="view" id="view-recommendations">
  <div class="sec-header">
    <div><div class="sec-title">Security Recommendations</div>
         <div class="sec-sub">AI-powered security advice for your system</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadRecommendations()">↺ Refresh</button>
  </div>
  <div id="recs-content">Loading…</div>
</section>

<!-- ── Monitoring ────────────────────────────────────────────────────── -->
<section class="view" id="view-monitoring">
  <div class="sec-header">
    <div><div class="sec-title">System Monitoring</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadMetrics()">↺ Refresh</button>
  </div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="mon-metrics">Metrics</button>
    <button class="tab-btn" data-tab="mon-procs">Processes</button>
  </div>
  <div id="tab-mon-metrics" class="tab-pane active">
    <div class="card-grid">
      <div class="card"><div class="card-title">CPU</div>
        <div class="card-value" id="mon-cpu">…</div></div>
      <div class="card"><div class="card-title">RAM Used</div>
        <div class="card-value" id="mon-ram">…</div>
        <div class="card-sub" id="mon-ram-sub">…</div></div>
    </div>
    <div id="mon-extra">Loading…</div>
  </div>
  <div id="tab-mon-procs" class="tab-pane">
    <div id="procs-content">Loading…</div></div>
</section>

<!-- ── Services ─────────────────────────────────────────────────────── -->
<section class="view" id="view-services">
  <div class="sec-header">
    <div><div class="sec-title">Windows Services</div></div>
    <div style="display:flex;gap:8px;align-items:center">
      <input class="input" id="svc-search" placeholder="Search services…" style="width:200px"
             oninput="filterServices()"/>
      <button class="btn btn-secondary btn-sm" onclick="loadServices()">↺ Refresh</button>
    </div>
  </div>
  <div id="services-content">Loading…</div>
</section>

<!-- ── Network ──────────────────────────────────────────────────────── -->
<section class="view" id="view-network">
  <div class="sec-header">
    <div><div class="sec-title">Network</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadNetwork()">↺ Refresh</button>
  </div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="net-adapters">Adapters</button>
    <button class="tab-btn" data-tab="net-connections">Connections</button>
    <button class="tab-btn" data-tab="net-stats">Stats</button>
  </div>
  <div id="tab-net-adapters"    class="tab-pane active"><div id="adapters-content">Loading…</div></div>
  <div id="tab-net-connections" class="tab-pane"><div id="connections-content">Loading…</div></div>
  <div id="tab-net-stats"       class="tab-pane"><div id="netstats-content">Loading…</div></div>
</section>

<!-- ── Firewall ──────────────────────────────────────────────────────── -->
<section class="view" id="view-firewall">
  <div class="sec-header">
    <div><div class="sec-title">Firewall Manager</div></div>
    <button class="btn btn-secondary btn-sm" onclick="loadFirewall()">↺ Refresh</button>
  </div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="fw-profiles">Profiles</button>
    <button class="tab-btn" data-tab="fw-rules">Rules</button>
  </div>
  <div id="tab-fw-profiles" class="tab-pane active"><div id="fw-profiles-content">Loading…</div></div>
  <div id="tab-fw-rules"    class="tab-pane"><div id="fw-rules-content">Loading…</div></div>
</section>

<!-- ── Security Tools ────────────────────────────────────────────────── -->
<section class="view" id="view-sectools">
  <div class="sec-header">
    <div><div class="sec-title">🔒 Local Security Tools</div>
         <div class="sec-sub">Network &amp; password testing — all local, no external services</div></div>
  </div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="st-portscan">Port Scanner</button>
    <button class="tab-btn" data-tab="st-password">Password</button>
    <button class="tab-btn" data-tab="st-hash">Hash</button>
    <button class="tab-btn" data-tab="st-ssl">SSL Check</button>
    <button class="tab-btn" data-tab="st-ping">Ping / Trace</button>
    <button class="tab-btn" data-tab="st-dns">DNS Lookup</button>
    <button class="tab-btn" data-tab="st-arp">ARP Scan</button>
  </div>

  <!-- Port Scanner -->
  <div id="tab-st-portscan" class="tab-pane active">
    <div class="field"><label>Host / IP</label>
      <div class="input-row">
        <input class="input" id="ps-host" placeholder="192.168.1.1 or example.com"/>
        <input class="input" id="ps-ports" placeholder="1-1024" style="max-width:130px"/>
        <button class="btn btn-primary" id="ps-run">Scan</button>
      </div></div>
    <div class="tool-result" id="ps-result">Enter a host and click Scan.</div>
    <div id="ps-table-wrap" style="display:none;margin-top:12px">
      <table class="tbl" id="ps-table">
        <thead><tr><th>Port</th><th>Service</th><th>Risk</th></tr></thead>
        <tbody></tbody>
      </table></div>
  </div>

  <!-- Password Strength -->
  <div id="tab-st-password" class="tab-pane">
    <div class="field"><label>Password</label>
      <div class="input-row">
        <input class="input" id="pw-input" type="password" placeholder="Enter password to test"/>
        <button class="btn btn-secondary" id="pw-toggle" style="white-space:nowrap">Show</button>
        <button class="btn btn-primary" id="pw-run">Check</button>
      </div></div>
    <div id="pw-result" style="display:none">
      <div style="display:flex;align-items:center;gap:12px;margin-bottom:8px">
        <span id="pw-level" style="font-size:18px;font-weight:700">—</span>
        <span id="pw-score" style="color:var(--muted);font-size:12px"></span>
        <span id="pw-entropy" style="color:var(--muted);font-size:12px"></span>
      </div>
      <div class="pw-bar-wrap"><div id="pw-bar" class="pw-bar" style="width:0%"></div></div>
      <div id="pw-checks" style="margin-top:10px;font-size:12px;font-family:monospace"></div>
      <div id="pw-issues" style="margin-top:8px"></div>
      <div id="pw-tips" style="margin-top:6px"></div>
    </div>
  </div>

  <!-- Hash Generator -->
  <div id="tab-st-hash" class="tab-pane">
    <div class="field"><label>Text / Data</label>
      <div class="input-row">
        <input class="input" id="hash-input" placeholder="Enter text to hash"/>
        <button class="btn btn-primary" id="hash-run">Generate</button>
      </div></div>
    <div id="hash-result" style="display:none;margin-top:10px" id="hash-rows"></div>
  </div>

  <!-- SSL Checker -->
  <div id="tab-st-ssl" class="tab-pane">
    <div class="field"><label>Hostname</label>
      <div class="input-row">
        <input class="input" id="ssl-host" placeholder="example.com"/>
        <input class="input" id="ssl-port" value="443" style="max-width:80px"/>
        <button class="btn btn-primary" id="ssl-run">Check</button>
      </div></div>
    <div class="tool-result" id="ssl-result">Enter a hostname and click Check.</div>
  </div>

  <!-- Ping / Traceroute -->
  <div id="tab-st-ping" class="tab-pane">
    <div class="field"><label>Host / IP</label>
      <div class="input-row">
        <input class="input" id="ping-host" placeholder="8.8.8.8 or example.com"/>
        <input class="input" id="ping-count" value="4" style="max-width:70px" title="Ping count"/>
        <button class="btn btn-primary" id="ping-run">Ping</button>
        <button class="btn btn-secondary" id="trace-run">Traceroute</button>
      </div></div>
    <div class="tool-result" id="ping-result">Enter a host and click Ping or Traceroute.</div>
  </div>

  <!-- DNS Lookup -->
  <div id="tab-st-dns" class="tab-pane">
    <div class="field"><label>Domain</label>
      <div class="input-row">
        <input class="input" id="dns-domain" placeholder="example.com"/>
        <button class="btn btn-primary" id="dns-run">Lookup</button>
      </div></div>
    <div class="tool-result" id="dns-result">Enter a domain and click Lookup.</div>
  </div>

  <!-- ARP Scan -->
  <div id="tab-st-arp" class="tab-pane">
    <div style="margin-bottom:10px">
      <button class="btn btn-primary" id="arp-run">Scan Local Network (ARP)</button>
      <span id="arp-note" style="margin-left:12px;color:var(--muted);font-size:12px"></span>
    </div>
    <div id="arp-table-wrap" style="display:none">
      <table class="tbl" id="arp-table">
        <thead><tr><th>IP Address</th><th>MAC Address</th><th>Type</th></tr></thead>
        <tbody></tbody>
      </table>
    </div>
  </div>
</section>

</main>

<script>
// ══════════════════════════════════════════════════════════════
// BRIDGE — talk to C# SecurityBridge via WebMessageReceived
// ══════════════════════════════════════════════════════════════
const pending = {};
let msgId = 0;

function api(endpoint, payload = {}) {
  return new Promise((resolve, reject) => {
    const id = String(++msgId);
    pending[id] = { resolve, reject };
    window.chrome.webview.postMessage(JSON.stringify({ id, endpoint, payload }));
  });
}

window.chrome.webview.addEventListener('message', e => {
  try {
    const msg = JSON.parse(e.data);
    if (msg.id && pending[msg.id]) {
      pending[msg.id].resolve(msg.data);
      delete pending[msg.id];
    }
  } catch {}
});

// ── Clock ─────────────────────────────────────────────────────
setInterval(() => {
  document.getElementById('nav-time').textContent =
    new Date().toLocaleString('en-GB', {hour:'2-digit',minute:'2-digit',second:'2-digit',
                                         day:'2-digit',month:'short',year:'numeric'});
}, 1000);

// ── Tab init ──────────────────────────────────────────────────
function initTabs(root) {
  root.querySelectorAll('.tab-bar').forEach(bar => {
    const container = bar.parentElement;
    const panes = Array.from(container.children).filter(el => el.classList.contains('tab-pane'));
    bar.querySelectorAll('.tab-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        bar.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        panes.forEach(p => p.classList.remove('active'));
        const t = document.getElementById('tab-' + btn.dataset.tab);
        if (t) t.classList.add('active');
        // Lazy loads
        const tab = btn.dataset.tab;
        if (tab === 'mon-procs')     loadProcesses();
        if (tab === 'ev-security')   loadEvSecurity();
        if (tab === 'ev-system')     loadEvSystem();
        if (tab === 'net-adapters')  loadAdapters();
        if (tab === 'net-connections') loadConnections();
        if (tab === 'net-stats')     loadNetStats();
        if (tab === 'fw-profiles')   loadFwProfiles();
        if (tab === 'fw-rules')      loadFwRules();
        if (tab === 'st-arp')        document.getElementById('arp-run').click();
      });
    });
  });
}
initTabs(document);

// ── Navigation ────────────────────────────────────────────────
document.querySelectorAll('.nav-item').forEach(item => {
  item.addEventListener('click', () => {
    const v = item.dataset.view;
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
    item.classList.add('active');
    document.querySelectorAll('.view').forEach(s => s.classList.remove('active'));
    const view = document.getElementById('view-' + v);
    if (view) view.classList.add('active');
    // Lazy load when navigating
    if (v === 'dashboard')       loadDashboard();
    if (v === 'defender')        loadDefender();
    if (v === 'events')          loadEvSecurity();
    if (v === 'recommendations') loadRecommendations();
    if (v === 'monitoring')      loadMetrics();
    if (v === 'services')        loadServices();
    if (v === 'network')         loadAdapters();
    if (v === 'firewall')        loadFwProfiles();
  });
});

// ── Helpers ───────────────────────────────────────────────────
function sev(s) {
  return s==='CRITICAL'?'badge-crit':s==='HIGH'?'badge-high':
         s==='MEDIUM'?'badge-warn':s==='ok'||s==='LOW'?'badge-ok':'badge-info';
}
function pct(n) { return `${n}%`; }
function toast(msg, type='info') {
  const t = document.createElement('div');
  t.style.cssText = `position:fixed;bottom:20px;right:20px;padding:10px 16px;
    border-radius:6px;font-size:12px;z-index:9999;
    background:var(--raised);border:1px solid var(--border);color:var(--text)`;
  t.textContent = msg;
  document.body.appendChild(t);
  setTimeout(() => t.remove(), 2500);
}

// ══════════════════════════════════════════════════════════════
// DASHBOARD
// ══════════════════════════════════════════════════════════════
async function loadDashboard() {
  document.getElementById('dash-updated').textContent = 'Refreshing…';
  const [sys, metrics, def] = await Promise.all([
    api('system/info'), api('system/metrics'), api('defender/status')
  ]);

  // Status cards
  document.getElementById('dc-status').innerHTML =
    `<span class="badge ${def.status==='protected'?'badge-ok':'badge-warn'}">${def.status?.toUpperCase()??'UNKNOWN'}</span>`;
  document.getElementById('dc-cpu').textContent = metrics.cpuPct + '%';
  document.getElementById('dc-ram').textContent = metrics.ramUsedGb + ' GB';
  document.getElementById('dc-ram-sub').textContent = `of ${metrics.ramTotalGb} GB (${metrics.ramPct}%)`;
  document.getElementById('dc-procs').textContent = '—';
  api('system/processes').then(p => {
    document.getElementById('dc-procs').textContent = p.count ?? '—';
  });

  // Sys info
  document.getElementById('dash-sysinfo').innerHTML = [
    ['Host', sys.hostname], ['User', sys.username + '@' + sys.domain],
    ['OS', sys.platform], ['CPU', sys.cpu],
    ['RAM', sys.ramTotalGb + ' GB'], ['Uptime', sys.uptime]
  ].map(([k,v]) =>
    `<div class="info-row"><span class="info-label">${k}</span><span class="info-val">${v??'—'}</span></div>`
  ).join('');

  // Defender status
  document.getElementById('dash-defender').innerHTML = [
    ['Antivirus', def.avName, def.avEnabled?'badge-ok':'badge-crit', def.avState],
    ['Real-time Protection', '', def.rtpEnabled?'badge-ok':'badge-crit', def.rtpEnabled?'enabled':'DISABLED'],
    ['Firewall', '', def.fwEnabled?'badge-ok':'badge-warn', def.fwEnabled?'enabled':'check profiles'],
    ['Last Scan', '', 'badge-info', def.lastScan||'Unknown'],
    ['Signatures', '', 'badge-info', def.sigDate||'Unknown']
  ].map(([k,_,cls,v]) =>
    `<div class="info-row"><span class="info-label">${k}</span>
     <span class="badge ${cls}">${v}</span></div>`
  ).join('');

  // Drives
  const drives = sys.drives ?? [];
  document.getElementById('dash-drives').innerHTML = drives.map(d =>
    `<div style="margin-bottom:10px">
       <div style="display:flex;justify-content:space-between;font-size:12px;margin-bottom:4px">
         <span>${d.name} <span style="color:var(--muted)">${d.type}</span></span>
         <span style="color:var(--muted)">${d.freeGb} GB free of ${d.totalGb} GB</span>
       </div>
       <div style="background:var(--raised);height:6px;border-radius:3px;overflow:hidden">
         <div style="height:100%;border-radius:3px;width:${d.usedPct}%;
           background:${d.usedPct>90?'var(--critical)':d.usedPct>70?'var(--medium)':'var(--accent)'}"></div>
       </div></div>`
  ).join('') || '<span style="color:var(--muted)">No drives found</span>';

  document.getElementById('dash-updated').textContent =
    'Last updated: ' + new Date().toLocaleTimeString();
}

// ══════════════════════════════════════════════════════════════
// DEFENDER
// ══════════════════════════════════════════════════════════════
async function loadDefender() {
  document.getElementById('defender-content').innerHTML =
    '<div class="spinner"></div> Loading…';
  const d = await api('defender/status');
  document.getElementById('defender-content').innerHTML = `
    <div class="card-grid">
      <div class="card ${d.avEnabled?'card-ok':'card-crit'}">
        <div class="card-title">Antivirus</div>
        <div class="card-value"><span class="badge ${d.avEnabled?'badge-ok':'badge-crit'}">${d.avState?.toUpperCase()}</span></div>
        <div class="card-sub">${d.avName}</div>
      </div>
      <div class="card ${d.rtpEnabled?'card-ok':'card-crit'}">
        <div class="card-title">Real-time Protection</div>
        <div class="card-value"><span class="badge ${d.rtpEnabled?'badge-ok':'badge-crit'}">${d.rtpEnabled?'ON':'OFF'}</span></div>
      </div>
      <div class="card">
        <div class="card-title">Last Quick Scan</div>
        <div class="card-value" style="font-size:13px">${d.lastScan||'Unknown'}</div>
      </div>
      <div class="card">
        <div class="card-title">Sig. Updated</div>
        <div class="card-value" style="font-size:13px">${d.sigDate||'Unknown'}</div>
      </div>
    </div>
    ${!d.avEnabled||!d.rtpEnabled?
      `<div class="alert alert-crit">⚠ Security Warning: Antivirus or real-time protection is disabled.</div>`:''}`;
}

async function startScan() {
  const btn = document.getElementById('scan-btn');
  btn.disabled = true; btn.textContent = '⏳ Starting…';
  const r = await api('defender/scan');
  btn.disabled = false; btn.textContent = '▶ Quick Scan';
  toast(r.ok ? 'Quick scan started' : 'Failed: ' + r.message, r.ok?'ok':'error');
}

// ══════════════════════════════════════════════════════════════
// EVENT LOGS
// ══════════════════════════════════════════════════════════════
async function loadEvents() { loadEvSecurity(); }

async function loadEvSecurity() {
  const el = document.getElementById('ev-security-content');
  el.innerHTML = '<div class="spinner"></div> Loading…';
  const d = await api('events/security');
  renderEvents(el, d);
}
async function loadEvSystem() {
  const el = document.getElementById('ev-system-content');
  el.innerHTML = '<div class="spinner"></div> Loading…';
  const d = await api('events/system');
  renderEvents(el, d);
}
function renderEvents(el, d) {
  if (d.error) { el.innerHTML = `<div class="alert alert-warn">${d.error}</div>`; return; }
  const evs = d.events ?? [];
  el.innerHTML = `<table class="tbl">
    <thead><tr><th>Time</th><th>Level</th><th>Source</th><th>Message</th></tr></thead>
    <tbody>${evs.map(e =>
      `<tr><td style="white-space:nowrap">${e.time}</td>
           <td><span class="badge ${e.level==='Error'?'badge-crit':e.level==='Warning'?'badge-warn':'badge-info'}">${e.level}</span></td>
           <td>${e.source}</td><td style="max-width:400px;overflow:hidden">${e.message}</td></tr>`
    ).join('')}</tbody></table>`;
}

// ══════════════════════════════════════════════════════════════
// RECOMMENDATIONS
// ══════════════════════════════════════════════════════════════
async function loadRecommendations() {
  const el = document.getElementById('recs-content');
  el.innerHTML = '<div class="spinner"></div> Analyzing system…';
  const d = await api('recommendations');
  const items = d.items ?? [];
  el.innerHTML = items.map(r =>
    `<div class="rec-item ${r.severity}">
       <div style="display:flex;align-items:center;gap:8px;margin-bottom:5px">
         <span class="badge ${sev(r.severity)}">${r.severity}</span>
         <strong style="font-size:13px">${r.title}</strong>
         <span style="color:var(--muted);font-size:11px;margin-left:auto">${r.category}</span>
       </div>
       <div style="color:var(--muted);font-size:12px">${r.description}</div>
     </div>`
  ).join('');
}

// ══════════════════════════════════════════════════════════════
// MONITORING
// ══════════════════════════════════════════════════════════════
async function loadMetrics() {
  const d = await api('system/metrics');
  document.getElementById('mon-cpu').textContent = d.cpuPct + '%';
  document.getElementById('mon-ram').textContent = d.ramUsedGb + ' GB';
  document.getElementById('mon-ram-sub').textContent = `of ${d.ramTotalGb} GB (${d.ramPct}%)`;
}

async function loadProcesses() {
  const el = document.getElementById('procs-content');
  el.innerHTML = '<div class="spinner"></div> Loading…';
  const d = await api('system/processes');
  const procs = (d.processes ?? []).slice(0, 60);
  el.innerHTML = `<table class="tbl">
    <thead><tr><th>PID</th><th>Name</th><th>Memory (MB)</th><th>Threads</th></tr></thead>
    <tbody>${procs.map(p =>
      `<tr><td>${p.pid}</td><td>${p.name}</td><td>${p.memMb}</td><td>${p.threads}</td></tr>`
    ).join('')}</tbody></table>`;
}

// ══════════════════════════════════════════════════════════════
// SERVICES
// ══════════════════════════════════════════════════════════════
let allServices = [];
async function loadServices() {
  const el = document.getElementById('services-content');
  el.innerHTML = '<div class="spinner"></div> Loading…';
  const d = await api('services/list');
  allServices = d.services ?? [];
  renderServices(allServices);
}
function filterServices() {
  const q = document.getElementById('svc-search').value.toLowerCase();
  renderServices(allServices.filter(s =>
    s.name.toLowerCase().includes(q) || s.displayName.toLowerCase().includes(q)));
}
function renderServices(list) {
  document.getElementById('services-content').innerHTML =
    `<table class="tbl"><thead>
       <tr><th>Name</th><th>Status</th><th>Start</th><th>Actions</th></tr>
     </thead><tbody>${list.map(s =>
       `<tr>
          <td><div>${s.displayName}</div><div style="color:var(--muted);font-size:10px">${s.name}</div></td>
          <td><span class="badge ${s.status==='Running'?'badge-ok':s.status==='Stopped'?'badge-crit':'badge-warn'}">${s.status}</span></td>
          <td style="color:var(--muted);font-size:11px">${s.startType}</td>
          <td>
            ${s.canStart?`<button class="btn btn-ok btn-sm" onclick="svcAction('start','${s.name}')">Start</button> `:''}
            ${s.canStop?`<button class="btn btn-danger btn-sm" onclick="svcAction('stop','${s.name}')">Stop</button> `:''}
            <button class="btn btn-secondary btn-sm" onclick="svcAction('restart','${s.name}')">↺</button>
          </td>
        </tr>`
     ).join('')}</tbody></table>`;
}
async function svcAction(action, name) {
  toast(`${action}ing ${name}…`);
  const r = await api(`services/${action}`, { name });
  toast(r.ok ? `${name}: ${r.status}` : `Failed: ${r.error}`, r.ok?'ok':'error');
  await loadServices();
}

// ══════════════════════════════════════════════════════════════
// NETWORK
// ══════════════════════════════════════════════════════════════
async function loadNetwork() { loadAdapters(); }
async function loadAdapters() {
  const el = document.getElementById('adapters-content');
  el.innerHTML = '<div class="spinner"></div>';
  const d = await api('network/adapters');
  const ads = d.adapters ?? [];
  el.innerHTML = ads.map(a => `
    <div class="card" style="margin-bottom:12px">
      <div style="display:flex;align-items:center;gap:8px;margin-bottom:8px">
        <strong>${a.name}</strong>
        <span class="badge ${a.status==='Up'?'badge-ok':'badge-crit'}">${a.status}</span>
        <span style="color:var(--muted);font-size:11px">${a.type}</span>
      </div>
      <div class="info-row"><span class="info-label">Description</span><span class="info-val">${a.description}</span></div>
      <div class="info-row"><span class="info-label">IP Addresses</span><span class="info-val">${a.ips.join(', ')||'—'}</span></div>
      <div class="info-row"><span class="info-label">Gateway</span><span class="info-val">${a.gateway||'—'}</span></div>
      <div class="info-row"><span class="info-label">DNS</span><span class="info-val">${a.dns||'—'}</span></div>
      <div class="info-row"><span class="info-label">MAC</span><span class="info-val" style="font-family:monospace">${a.mac||'—'}</span></div>
    </div>`
  ).join('') || '<span style="color:var(--muted)">No adapters found</span>';
}
async function loadConnections() {
  const el = document.getElementById('connections-content');
  el.innerHTML = '<div class="spinner"></div>';
  const d = await api('network/connections');
  const cs = d.connections ?? [];
  el.innerHTML = `<table class="tbl">
    <thead><tr><th>Protocol</th><th>Local</th><th>Remote</th><th>State</th></tr></thead>
    <tbody>${cs.map(c =>
      `<tr><td>${c.proto}</td><td>${c.local}</td><td>${c.remote}</td>
           <td><span class="badge ${c.state==='Established'?'badge-ok':c.state==='Listen'||c.state==='LISTEN'?'badge-info':'badge-warn'}">${c.state}</span></td></tr>`
    ).join('')}</tbody></table>`;
}
async function loadNetStats() {
  const el = document.getElementById('netstats-content');
  const d = await api('network/stats');
  if (d.error) { el.innerHTML = `<div class="alert alert-warn">${d.error}</div>`; return; }
  el.innerHTML = [
    ['Active TCP Connections', d.tcpActive],
    ['Failed Connection Attempts', d.tcpInFailed],
    ['Packets Received', d.packetsReceived],
    ['Packets Sent', d.packetsSent],
    ['Packet Errors', d.errors]
  ].map(([k,v]) =>
    `<div class="info-row"><span class="info-label">${k}</span><span class="info-val">${v??'—'}</span></div>`
  ).join('');
}

// ══════════════════════════════════════════════════════════════
// FIREWALL
// ══════════════════════════════════════════════════════════════
async function loadFirewall() { loadFwProfiles(); }
async function loadFwProfiles() {
  const el = document.getElementById('fw-profiles-content');
  el.innerHTML = '<div class="spinner"></div>';
  const d = await api('firewall/profiles');
  if (d.error) { el.innerHTML = `<div class="alert alert-warn">${d.error}</div>`; return; }
  // d.raw is JSON array from PowerShell
  try {
    const profiles = JSON.parse(d.raw);
    const arr = Array.isArray(profiles) ? profiles : [profiles];
    el.innerHTML = arr.map(p => `
      <div class="card" style="margin-bottom:10px;display:flex;align-items:center;justify-content:space-between">
        <div>
          <strong>${p.Name}</strong>
          <span class="badge ${p.Enabled?'badge-ok':'badge-crit'}" style="margin-left:8px">${p.Enabled?'Enabled':'Disabled'}</span>
        </div>
        <div style="color:var(--muted);font-size:11px">
          In: ${p.DefaultInboundAction} | Out: ${p.DefaultOutboundAction}
        </div>
        <button class="btn btn-sm ${p.Enabled?'btn-danger':'btn-ok'}"
          onclick="toggleFw('${p.Name}',${!p.Enabled})">
          ${p.Enabled?'Disable':'Enable'}
        </button>
      </div>`).join('');
  } catch {
    el.innerHTML = `<div class="tool-result">${d.raw||'No data'}</div>`;
  }
}
async function loadFwRules() {
  const el = document.getElementById('fw-rules-content');
  el.innerHTML = '<div class="spinner"></div>';
  const d = await api('firewall/rules');
  if (d.error) { el.innerHTML = `<div class="alert alert-warn">${d.error}</div>`; return; }
  try {
    const rules = JSON.parse(d.raw);
    const arr = Array.isArray(rules) ? rules : [rules];
    el.innerHTML = `<table class="tbl">
      <thead><tr><th>Name</th><th>Direction</th><th>Action</th><th>Profile</th><th>Protocol</th></tr></thead>
      <tbody>${arr.map(r =>
        `<tr><td>${r.DisplayName}</td>
             <td><span class="badge ${r.Direction==='Inbound'?'badge-warn':'badge-info'}">${r.Direction}</span></td>
             <td><span class="badge ${r.Action==='Allow'?'badge-ok':'badge-crit'}">${r.Action}</span></td>
             <td>${r.Profile}</td><td>${r.Protocol}</td></tr>`
      ).join('')}</tbody></table>`;
  } catch {
    el.innerHTML = `<div class="tool-result">${d.raw||'No rules'}</div>`;
  }
}
async function toggleFw(profile, enable) {
  const r = await api('firewall/toggle', { profile, enable });
  toast(r.ok ? `${profile} ${enable?'enabled':'disabled'}` : `Failed: ${r.error}`);
  loadFwProfiles();
}

// ══════════════════════════════════════════════════════════════
// SECURITY TOOLS
// ══════════════════════════════════════════════════════════════

// Port Scanner
document.getElementById('ps-run').addEventListener('click', async () => {
  const host  = document.getElementById('ps-host').value.trim();
  const ports = document.getElementById('ps-ports').value.trim() || '1-1024';
  const res   = document.getElementById('ps-result');
  const wrap  = document.getElementById('ps-table-wrap');
  if (!host) { res.textContent = 'Enter a host'; return; }
  res.className = 'tool-result running'; res.textContent = `Scanning ${host}:${ports}…`;
  wrap.style.display = 'none';
  const d = await api('sectools/portscan', { host, ports });
  if (d.error) { res.className='tool-result error'; res.textContent=d.error; return; }
  res.className = 'tool-result ok';
  res.textContent = `Host: ${d.host} (${d.ip})\nScanned: ${d.scanned} | Open: ${d.open} | Closed: ${d.closed}`;
  if (d.ports?.length > 0) {
    wrap.style.display = '';
    const tb = document.querySelector('#ps-table tbody'); tb.innerHTML = '';
    d.ports.forEach(p => {
      const risk = p.risk==='HIGH'?'badge-crit':p.risk==='MEDIUM'?'badge-high':'badge-ok';
      tb.insertAdjacentHTML('beforeend',
        `<tr><td>${p.port}</td><td>${p.service}</td><td><span class="badge ${risk}">${p.risk}</span></td></tr>`);
    });
  }
});

// Password
document.getElementById('pw-toggle').addEventListener('click', () => {
  const inp = document.getElementById('pw-input');
  const btn = document.getElementById('pw-toggle');
  inp.type = inp.type==='password'?'text':'password';
  btn.textContent = inp.type==='password'?'Show':'Hide';
});
document.getElementById('pw-run').addEventListener('click', async () => {
  const pw = document.getElementById('pw-input').value;
  if (!pw) return;
  const d = await api('sectools/password', { password: pw });
  const colorMap = {critical:'var(--critical)',high:'var(--high)',medium:'var(--medium)',low:'var(--low)'};
  const c = colorMap[d.color]||'var(--accent)';
  document.getElementById('pw-result').style.display = 'block';
  document.getElementById('pw-level').textContent  = d.level;
  document.getElementById('pw-level').style.color  = c;
  document.getElementById('pw-score').textContent  = `Score: ${d.score}/10`;
  document.getElementById('pw-entropy').textContent= `Entropy: ${d.entropy_bits} bits`;
  document.getElementById('pw-bar').style.width    = (d.score/10*100)+'%';
  document.getElementById('pw-bar').style.background = c;
  document.getElementById('pw-checks').innerHTML   = [
    ['Lowercase',d.has_lower],['Uppercase',d.has_upper],
    ['Numbers',d.has_digit],['Symbols',d.has_special],[`Length (${d.length})`,d.length>=12]
  ].map(([l,v])=>`<span style="color:${v?'var(--low)':'var(--critical)'}">  ${v?'✓':'✗'} ${l}</span><br>`).join('');
  document.getElementById('pw-issues').innerHTML = (d.issues||[]).map(i=>
    `<div class="alert alert-crit" style="padding:4px 10px;font-size:11px;margin-bottom:4px">⚠ ${i}</div>`).join('');
  document.getElementById('pw-tips').innerHTML = (d.tips||[]).map(t=>
    `<div style="color:var(--muted);font-size:11px">→ ${t}</div>`).join('');
});

// Hash
document.getElementById('hash-run').addEventListener('click', async () => {
  const text = document.getElementById('hash-input').value;
  if (!text) return;
  const d = await api('sectools/hash', { text });
  const el = document.getElementById('hash-result');
  el.style.display = 'block';
  el.innerHTML = Object.entries(d.hashes||{}).map(([a,h]) =>
    `<div class="info-row" style="align-items:center">
       <span class="info-label" style="text-transform:uppercase;color:var(--accent2)">${a}</span>
       <span class="info-val" style="font-family:monospace;font-size:11px">${h}</span>
       <button class="copy-btn" onclick="navigator.clipboard.writeText('${h}');toast('Copied')">Copy</button>
     </div>`).join('');
});

// SSL
document.getElementById('ssl-run').addEventListener('click', async () => {
  const host = document.getElementById('ssl-host').value.trim().replace(/^https?:\/\//,'').split('/')[0];
  const port = parseInt(document.getElementById('ssl-port').value)||443;
  const res  = document.getElementById('ssl-result');
  if (!host) { res.textContent='Enter hostname'; return; }
  res.className='tool-result running'; res.textContent=`Checking SSL for ${host}:${port}…`;
  const d = await api('sectools/ssl', { host, port });
  if (!d.valid) { res.className='tool-result error'; res.textContent=`❌ SSL Error\n${d.error}`; return; }
  res.className = d.expired?'tool-result error':d.expiring_soon?'tool-result':'tool-result ok';
  res.textContent = [
    `Host:        ${d.host}`,
    `Subject CN:  ${d.subject_cn}`,
    `Issuer:      ${d.issuer_org}`,
    `Valid From:  ${d.not_before}`,
    `Expires:     ${d.not_after}  (${d.days_remaining} days)`,
    `Thumbprint:  ${d.thumbprint}`,
    `SANs:        ${(d.sans||[]).join(', ')}`
  ].join('\n');
});

// Ping / Traceroute
async function runPing(trace) {
  const host  = document.getElementById('ping-host').value.trim();
  const count = parseInt(document.getElementById('ping-count').value)||4;
  const res   = document.getElementById('ping-result');
  if (!host) { res.textContent='Enter host'; return; }
  res.className='tool-result running'; res.textContent=trace?`Tracing route to ${host}…`:`Pinging ${host}…`;
  const d = await api(trace?'sectools/traceroute':'sectools/ping',
                      trace?{host}:{host,count});
  if (d.error) { res.className='tool-result error'; res.textContent=d.error; return; }
  if (trace) { res.className='tool-result ok'; res.textContent=d.output; return; }
  res.className = d.reachable?'tool-result ok':'tool-result error';
  res.textContent = d.reachable
    ? `${host} is reachable\nMin: ${d.min_ms}ms  Max: ${d.max_ms}ms  Avg: ${d.avg_ms}ms\nPacket loss: ${d.packet_loss_pct}%`
    : `${host} is unreachable`;
}
document.getElementById('ping-run').addEventListener('click',  ()=>runPing(false));
document.getElementById('trace-run').addEventListener('click', ()=>runPing(true));

// DNS
document.getElementById('dns-run').addEventListener('click', async () => {
  const domain = document.getElementById('dns-domain').value.trim();
  const res    = document.getElementById('dns-result');
  if (!domain) { res.textContent='Enter domain'; return; }
  res.className='tool-result running'; res.textContent='Querying DNS…';
  const d = await api('sectools/dns', { domain });
  const lines = [`Domain: ${d.domain}`, ''];
  for (const [type, val] of Object.entries(d.records||{})) {
    const vals = Array.isArray(val)?val:[val];
    if (vals.length) lines.push(`${type.padEnd(6)} ${vals.join(', ')}`);
  }
  res.className='tool-result ok'; res.textContent=lines.join('\n');
});

// ARP
document.getElementById('arp-run').addEventListener('click', async () => {
  const note = document.getElementById('arp-note');
  const wrap = document.getElementById('arp-table-wrap');
  note.textContent='Scanning…'; wrap.style.display='none';
  const d = await api('sectools/arp');
  if (d.error) { note.textContent='Error: '+d.error; return; }
  note.textContent = `${d.count} host(s) found`;
  if (d.count > 0) {
    wrap.style.display='';
    const tb = document.querySelector('#arp-table tbody'); tb.innerHTML='';
    (d.hosts||[]).forEach(h =>
      tb.insertAdjacentHTML('beforeend',
        `<tr><td>${h.ip}</td><td style="font-family:monospace">${h.mac}</td><td>${h.type}</td></tr>`));
  }
});

// ── Initial load ───────────────────────────────────────────────
loadDashboard();
document.getElementById('nav-status').textContent = 'v4.0 · Running';
</script>
</body>
</html>
""";
    }
}
