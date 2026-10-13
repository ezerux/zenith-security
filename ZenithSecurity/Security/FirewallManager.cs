using System;
using System.Threading.Tasks;
using ZenithSecurity.Security.Helpers;

namespace ZenithSecurity.Security
{
    public class FirewallManager
    {
        public async Task<object> GetProfilesAsync() => await Task.Run(() =>
        {
            try
            {
                var json = PowerShellHelper.Run(
                    "Get-NetFirewallProfile | Select-Object Name,Enabled,DefaultInboundAction,DefaultOutboundAction | ConvertTo-Json");
                return new { raw = json, source = "powershell" };
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        });

        public async Task<object> GetRulesAsync() => await Task.Run(() =>
        {
            try
            {
                var json = PowerShellHelper.Run(
                    "Get-NetFirewallRule | Where-Object {$_.Enabled -eq 'True'} | " +
                    "Select-Object -First 50 DisplayName,Direction,Action,Profile,Protocol,Enabled | " +
                    "ConvertTo-Json");
                return new { raw = json, source = "powershell" };
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        });

        public async Task<object> ToggleProfileAsync(string profile, bool enable) => await Task.Run(() =>
        {
            try
            {
                var action = enable ? "True" : "False";
                PowerShellHelper.Run(
                    $"Set-NetFirewallProfile -Profile {profile} -Enabled {action}");
                return new { ok = true, profile, enabled = enable };
            }
            catch (Exception ex)
            {
                return new { ok = false, error = ex.Message };
            }
        });
    }
}
