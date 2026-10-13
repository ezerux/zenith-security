using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZenithSecurity.Security
{
    public class SystemInfo
    {
        public async Task<object> GetSystemInfoAsync() => await Task.Run(() =>
        {
            var os = Environment.OSVersion;
            string cpuName = "", gpuName = "", mbName = "";

            try
            {
                using var cpuSearch = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (ManagementObject obj in cpuSearch.Get())
                    cpuName = obj["Name"]?.ToString()?.Trim() ?? "";
            }
            catch { cpuName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown"; }

            try
            {
                using var gpuSearch = new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController");
                foreach (ManagementObject obj in gpuSearch.Get())
                { gpuName = obj["Caption"]?.ToString() ?? ""; break; }
            }
            catch { gpuName = "Unknown"; }

            ulong totalRam = 0;
            try
            {
                using var cs = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in cs.Get())
                    totalRam = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
            }
            catch { }

            var drives = new List<object>();
            foreach (var di in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    if (di.IsReady)
                        drives.Add(new
                        {
                            name       = di.Name,
                            type       = di.DriveType.ToString(),
                            totalGb    = Math.Round(di.TotalSize / 1073741824.0, 1),
                            freeGb     = Math.Round(di.AvailableFreeSpace / 1073741824.0, 1),
                            usedPct    = (int)((1.0 - (double)di.AvailableFreeSpace / di.TotalSize) * 100)
                        });
                }
                catch { }
            }

            return new
            {
                hostname  = Environment.MachineName,
                username  = Environment.UserName,
                domain    = Environment.UserDomainName,
                os        = os.VersionString,
                osVersion = os.Version.ToString(),
                platform  = RuntimeInformation.OSDescription,
                cpu       = cpuName,
                gpu       = gpuName,
                ramTotalGb= Math.Round(totalRam / 1073741824.0, 2),
                cpuCount  = Environment.ProcessorCount,
                is64bit   = Environment.Is64BitOperatingSystem,
                uptime    = GetUptime(),
                drives
            };
        });

        public async Task<object> GetMetricsAsync() => await Task.Run(() =>
        {
            float cpu = 0;
            try
            {
                using var pc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                pc.NextValue();
                System.Threading.Thread.Sleep(200);
                cpu = pc.NextValue();
            }
            catch { }

            ulong availRam = 0, totalRam = 0;
            try
            {
                using var cs = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in cs.Get())
                    totalRam = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                using var os = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in os.Get())
                    availRam = Convert.ToUInt64(obj["FreePhysicalMemory"]) * 1024;
            }
            catch { }

            long usedRam = (long)(totalRam - availRam);
            int ramPct = totalRam > 0 ? (int)((double)usedRam / totalRam * 100) : 0;

            return new
            {
                cpuPct    = Math.Round(cpu, 1),
                ramUsedGb = Math.Round(usedRam  / 1073741824.0, 2),
                ramTotalGb= Math.Round(totalRam / 1073741824.0, 2),
                ramPct,
                timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
        });

        public async Task<object> GetProcessesAsync() => await Task.Run(() =>
        {
            var procs = new List<object>();
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    procs.Add(new
                    {
                        pid      = p.Id,
                        name     = p.ProcessName,
                        memMb    = Math.Round(p.WorkingSet64 / 1048576.0, 1),
                        threads  = p.Threads.Count
                    });
                }
                catch { }
            }
            procs.Sort((a, b) =>
            {
                double ma = (double)a.GetType().GetProperty("memMb")!.GetValue(a)!;
                double mb2 = (double)b.GetType().GetProperty("memMb")!.GetValue(b)!;
                return mb2.CompareTo(ma);
            });
            return new { count = procs.Count, processes = procs };
        });

        private static string GetUptime()
        {
            try
            {
                var ts = TimeSpan.FromMilliseconds(Environment.TickCount64);
                return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
            }
            catch { return "Unknown"; }
        }
    }
}
