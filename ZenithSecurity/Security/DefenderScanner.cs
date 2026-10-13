using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using ZenithSecurity.Security.Helpers;

namespace ZenithSecurity.Security
{
    public class DefenderScanner
    {
        public async Task<object> GetStatusAsync() => await Task.Run(() =>
        {
            // Windows Security Center via WMI
            bool avEnabled = false, fwEnabled = false;
            string avName = "Windows Defender", avState = "unknown";

            try
            {
                using var scope = new ManagementScope(@"\\.\root\SecurityCenter2");
                scope.Connect();
                using var avQ = new ManagementObjectSearcher(scope,
                    new ObjectQuery("SELECT * FROM AntiVirusProduct"));
                foreach (ManagementObject o in avQ.Get())
                {
                    avName    = o["displayName"]?.ToString() ?? avName;
                    int state = Convert.ToInt32(o["productState"]);
                    // Bit 12-19 = AV status: 0x10 = enabled, 0x00 = disabled
                    avEnabled = ((state >> 12) & 0xF) == 1;
                    avState   = avEnabled ? "enabled" : "disabled";
                    break;
                }
            }
            catch { avState = "unknown"; }

            // Firewall state
            var fwProfiles = new Dictionary<string, bool>();
            try
            {
                var result = PowerShellHelper.Run(
                    "Get-NetFirewallProfile | Select-Object Name,Enabled | ConvertTo-Json");
                if (!string.IsNullOrEmpty(result))
                {
                    // Parse basic enabled status
                    fwEnabled = result.Contains("\"Enabled\": true", StringComparison.OrdinalIgnoreCase)
                             || result.Contains("\"Enabled\":true",  StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { }

            // Real-time protection via PowerShell
            bool rtpEnabled = false;
            try
            {
                var rtp = PowerShellHelper.Run(
                    "(Get-MpComputerStatus).RealTimeProtectionEnabled");
                rtpEnabled = rtp.Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
            }
            catch { }

            // Last scan time
            string lastScan = "Unknown";
            try
            {
                var ls = PowerShellHelper.Run("(Get-MpComputerStatus).QuickScanEndTime");
                lastScan = ls.Trim();
            }
            catch { }

            // Signature dates
            string sigDate = "Unknown";
            try
            {
                var sd = PowerShellHelper.Run("(Get-MpComputerStatus).AntivirusSignatureLastUpdated");
                sigDate = sd.Trim();
            }
            catch { }

            return new
            {
                avName,
                avEnabled,
                avState,
                rtpEnabled,
                fwEnabled,
                lastScan,
                sigDate,
                status = (avEnabled && rtpEnabled) ? "protected" : "warning"
            };
        });

        public async Task<object> GetThreatHistoryAsync() => await Task.Run(() =>
        {
            var threats = new List<object>();
            try
            {
                var json = PowerShellHelper.Run(
                    "Get-MpThreatDetection | Select-Object -First 20 " +
                    "ThreatID,ActionSuccess,ProcessName,Resources,InitialDetectionTime " +
                    "| ConvertTo-Json");
                // Return raw JSON for JS to parse
                return new { raw = json, source = "powershell" };
            }
            catch (Exception ex)
            {
                return new { threats, error = ex.Message };
            }
        });

        public async Task<object> QuickScanAsync() => await Task.Run(() =>
        {
            try
            {
                var r = PowerShellHelper.Run("Start-MpScan -ScanType QuickScan");
                return new { ok = true, message = "Quick scan started" };
            }
            catch (Exception ex)
            {
                return new { ok = false, message = ex.Message };
            }
        });
    }
}
