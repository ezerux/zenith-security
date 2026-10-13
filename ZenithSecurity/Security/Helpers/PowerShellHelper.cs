using System.Diagnostics;
using System.Text;

namespace ZenithSecurity.Security.Helpers
{
    public static class PowerShellHelper
    {
        public static string Run(string command, int timeoutMs = 15000)
        {
            var psi = new ProcessStartInfo
            {
                FileName               = "powershell.exe",
                Arguments              = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{command.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using var p = Process.Start(psi)!;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit(timeoutMs);
            return output.Trim();
        }

        public static string RunScript(string script, int timeoutMs = 30000)
        {
            var psi = new ProcessStartInfo
            {
                FileName               = "powershell.exe",
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            psi.ArgumentList.Add("-NoProfile");
            psi.ArgumentList.Add("-NonInteractive");
            psi.ArgumentList.Add("-ExecutionPolicy");
            psi.ArgumentList.Add("Bypass");
            psi.ArgumentList.Add("-Command");
            psi.ArgumentList.Add("-");

            using var p = Process.Start(psi)!;
            p.StandardInput.WriteLine(script);
            p.StandardInput.Close();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit(timeoutMs);
            return output.Trim();
        }
    }
}
