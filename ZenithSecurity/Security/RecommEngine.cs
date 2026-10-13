using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZenithSecurity.Security.Helpers;

namespace ZenithSecurity.Security
{
    public class RecommEngine
    {
        public async Task<object> GetRecommendationsAsync() => await Task.Run(() =>
        {
            var items = new List<object>();

            // Check Windows Update
            try
            {
                var wu = PowerShellHelper.Run(
                    "(New-Object -ComObject Microsoft.Update.Session).CreateUpdateSearcher().Search('IsInstalled=0').Updates.Count");
                if (int.TryParse(wu.Trim(), out int count) && count > 0)
                    items.Add(Rec("HIGH", "Windows Updates Pending",
                        $"{count} update(s) pending. Install updates to protect against vulnerabilities.",
                        "Windows Update", "update"));
            }
            catch { }

            // Check firewall
            try
            {
                var fw = PowerShellHelper.Run(
                    "Get-NetFirewallProfile | Where-Object {$_.Enabled -eq $false} | Select-Object -ExpandProperty Name");
                if (!string.IsNullOrEmpty(fw.Trim()))
                    items.Add(Rec("HIGH", "Firewall Profile Disabled",
                        $"Firewall profile(s) disabled: {fw.Trim()}. Enable all profiles for protection.",
                        "Firewall", "shield"));
            }
            catch { }

            // Check Defender RTP
            try
            {
                var rtp = PowerShellHelper.Run("(Get-MpComputerStatus).RealTimeProtectionEnabled");
                if (rtp.Trim().Equals("False", StringComparison.OrdinalIgnoreCase))
                    items.Add(Rec("CRITICAL", "Real-Time Protection Disabled",
                        "Windows Defender real-time protection is off. Malware can run undetected.",
                        "Windows Defender", "bug"));
            }
            catch { }

            // Check guest account
            try
            {
                var guest = PowerShellHelper.Run(
                    "(Get-LocalUser -Name Guest).Enabled");
                if (guest.Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
                    items.Add(Rec("MEDIUM", "Guest Account Enabled",
                        "The Guest account is enabled. Disable it to reduce attack surface.",
                        "User Accounts", "user"));
            }
            catch { }

            // Check Remote Desktop
            try
            {
                var rdp = PowerShellHelper.Run(
                    @"(Get-ItemProperty 'HKLM:\System\CurrentControlSet\Control\Terminal Server').fDenyTSConnections");
                if (rdp.Trim() == "0")
                    items.Add(Rec("MEDIUM", "Remote Desktop Enabled",
                        "RDP is enabled (port 3389). Disable if not needed or restrict with firewall rules.",
                        "Remote Access", "monitor"));
            }
            catch { }

            // Check AutoRun
            try
            {
                var ar = PowerShellHelper.Run(
                    @"(Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer' -ErrorAction SilentlyContinue).NoDriveTypeAutoRun");
                if (string.IsNullOrEmpty(ar.Trim()) || ar.Trim() == "0")
                    items.Add(Rec("LOW", "AutoRun May Be Enabled",
                        "AutoRun for removable media may be enabled. Consider disabling to prevent USB attacks.",
                        "System Policy", "disc"));
            }
            catch { }

            if (items.Count == 0)
                items.Add(Rec("INFO", "System Looks Good",
                    "No critical security issues found. Continue monitoring regularly.",
                    "All Systems", "check-circle"));

            return new { count = items.Count, items };
        });

        private static object Rec(string severity, string title, string desc, string category, string icon) =>
            new { severity, title, description = desc, category, icon };
    }
}
