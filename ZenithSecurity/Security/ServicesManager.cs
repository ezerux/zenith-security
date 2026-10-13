using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ZenithSecurity.Security
{
    public class ServicesManager
    {
        private static readonly HashSet<string> SecurityServices = new(StringComparer.OrdinalIgnoreCase)
        {
            "WinDefend","MpsSvc","EventLog","WSearch","Spooler","wuauserv",
            "BITS","CryptSvc","W32Time","Netlogon","RemoteRegistry","TermService",
            "Schedule","SamSs","LanmanServer","LanmanWorkstation","Dnscache"
        };

        public async Task<object> GetServicesAsync() => await Task.Run(() =>
        {
            var list = new List<object>();
            try
            {
                var services = ServiceController.GetServices();
                foreach (var svc in services)
                {
                    bool isSecurity = SecurityServices.Contains(svc.ServiceName);
                    list.Add(new
                    {
                        name        = svc.ServiceName,
                        displayName = svc.DisplayName,
                        status      = svc.Status.ToString(),
                        canStart    = svc.Status == ServiceControllerStatus.Stopped,
                        canStop     = svc.Status == ServiceControllerStatus.Running && svc.CanStop,
                        canPause    = svc.CanPauseAndContinue,
                        isSecurity,
                        startType   = GetStartType(svc)
                    });
                }
            }
            catch (Exception ex)
            {
                return new { error = ex.Message, services = list };
            }
            return new { count = list.Count, services = list };
        });

        public async Task<object> StartServiceAsync(string name) => await Task.Run(() =>
        {
            try
            {
                using var svc = new ServiceController(name);
                svc.Start();
                svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                return new { ok = true, name, status = "Running" };
            }
            catch (Exception ex) { return new { ok = false, error = ex.Message }; }
        });

        public async Task<object> StopServiceAsync(string name) => await Task.Run(() =>
        {
            try
            {
                using var svc = new ServiceController(name);
                svc.Stop();
                svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                return new { ok = true, name, status = "Stopped" };
            }
            catch (Exception ex) { return new { ok = false, error = ex.Message }; }
        });

        public async Task<object> RestartServiceAsync(string name) => await Task.Run(() =>
        {
            try
            {
                using var svc = new ServiceController(name);
                if (svc.Status == ServiceControllerStatus.Running)
                {
                    svc.Stop();
                    svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
                svc.Start();
                svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                return new { ok = true, name, status = "Running" };
            }
            catch (Exception ex) { return new { ok = false, error = ex.Message }; }
        });

        private static string GetStartType(ServiceController svc)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    $@"SYSTEM\CurrentControlSet\Services\{svc.ServiceName}");
                if (key?.GetValue("Start") is int st)
                    return st switch { 2 => "Auto", 3 => "Manual", 4 => "Disabled", _ => "Unknown" };
            }
            catch { }
            return "Unknown";
        }
    }
}
