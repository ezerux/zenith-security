using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZenithSecurity.Security
{
    /// <summary>
    /// Routes all API requests from the WebView HTML to the appropriate security module.
    /// </summary>
    public class SecurityBridge : IDisposable
    {
        private readonly SystemInfo      _sysInfo      = new();
        private readonly DefenderScanner _defender     = new();
        private readonly FirewallManager _firewall     = new();
        private readonly ServicesManager _services     = new();
        private readonly NetworkScanner  _network      = new();
        private readonly SecurityTools   _secTools     = new();
        private readonly EventLogReader  _eventLog     = new();
        private readonly RecommEngine    _recomm       = new();

        public async Task<object> HandleAsync(string endpoint, JsonElement payload)
        {
            return endpoint switch
            {
                // ── System ────────────────────────────────────────────────────
                "system/info"         => await _sysInfo.GetSystemInfoAsync(),
                "system/metrics"      => await _sysInfo.GetMetricsAsync(),
                "system/processes"    => await _sysInfo.GetProcessesAsync(),

                // ── Defender ──────────────────────────────────────────────────
                "defender/status"     => await _defender.GetStatusAsync(),
                "defender/threats"    => await _defender.GetThreatHistoryAsync(),
                "defender/scan"       => await _defender.QuickScanAsync(),

                // ── Firewall ──────────────────────────────────────────────────
                "firewall/profiles"   => await _firewall.GetProfilesAsync(),
                "firewall/rules"      => await _firewall.GetRulesAsync(),
                "firewall/toggle"     => await _firewall.ToggleProfileAsync(
                                            GetStr(payload, "profile"),
                                            GetBool(payload, "enable")),

                // ── Services ──────────────────────────────────────────────────
                "services/list"       => await _services.GetServicesAsync(),
                "services/start"      => await _services.StartServiceAsync(GetStr(payload,"name")),
                "services/stop"       => await _services.StopServiceAsync(GetStr(payload,"name")),
                "services/restart"    => await _services.RestartServiceAsync(GetStr(payload,"name")),

                // ── Network ───────────────────────────────────────────────────
                "network/adapters"    => await _network.GetAdaptersAsync(),
                "network/connections" => await _network.GetConnectionsAsync(),
                "network/stats"       => await _network.GetStatsAsync(),

                // ── Event Logs ────────────────────────────────────────────────
                "events/security"     => await _eventLog.GetSecurityEventsAsync(),
                "events/system"       => await _eventLog.GetSystemEventsAsync(),

                // ── Security Tools ────────────────────────────────────────────
                "sectools/portscan"   => await _secTools.PortScanAsync(
                                            GetStr(payload,"host"), GetStr(payload,"ports","1-1024")),
                "sectools/password"   => _secTools.PasswordStrength(GetStr(payload,"password")),
                "sectools/hash"       => _secTools.GenerateHash(GetStr(payload,"text")),
                "sectools/ssl"        => await _secTools.CheckSslAsync(
                                            GetStr(payload,"host"), GetInt(payload,"port",443)),
                "sectools/ping"       => await _secTools.PingAsync(
                                            GetStr(payload,"host"), GetInt(payload,"count",4)),
                "sectools/traceroute" => await _secTools.TracerouteAsync(GetStr(payload,"host")),
                "sectools/dns"        => await _secTools.DnsLookupAsync(GetStr(payload,"domain")),
                "sectools/arp"        => await _secTools.ArpScanAsync(),

                // ── Recommendations ───────────────────────────────────────────
                "recommendations"     => await _recomm.GetRecommendationsAsync(),

                _ => new { error = $"Unknown endpoint: {endpoint}" }
            };
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        private static string GetStr(JsonElement el, string key, string def = "")
        {
            try { return el.GetProperty(key).GetString() ?? def; } catch { return def; }
        }
        private static bool GetBool(JsonElement el, string key, bool def = false)
        {
            try { return el.GetProperty(key).GetBoolean(); } catch { return def; }
        }
        private static int GetInt(JsonElement el, string key, int def = 0)
        {
            try { return el.GetProperty(key).GetInt32(); } catch { return def; }
        }

        public void Dispose()
        {
            _secTools.Dispose();
        }
    }
}
