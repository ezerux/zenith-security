using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenithSecurity.Security.Helpers;

namespace ZenithSecurity.Security
{
    public class SecurityTools : IDisposable
    {
        private static readonly Dictionary<int, string> WellKnown = new()
        {
            {21,"FTP"},{22,"SSH"},{23,"Telnet"},{25,"SMTP"},{53,"DNS"},
            {80,"HTTP"},{110,"POP3"},{143,"IMAP"},{443,"HTTPS"},{445,"SMB"},
            {1433,"MSSQL"},{1521,"Oracle"},{3306,"MySQL"},{3389,"RDP"},
            {5432,"PostgreSQL"},{5900,"VNC"},{6379,"Redis"},{8080,"HTTP-Alt"},
            {8443,"HTTPS-Alt"},{9200,"Elasticsearch"},{27017,"MongoDB"}
        };
        private static readonly Dictionary<int, string> HighRisk = new()
        {
            {23,"HIGH"},{445,"HIGH"},{6379,"HIGH"},{27017,"HIGH"},{9200,"HIGH"}
        };
        private static readonly Dictionary<int, string> MedRisk = new()
        {
            {21,"MEDIUM"},{3389,"MEDIUM"},{5900,"MEDIUM"},{1521,"MEDIUM"}
        };

        // ── Port Scanner ───────────────────────────────────────────────────────
        public async Task<object> PortScanAsync(string host, string portRange = "1-1024")
        {
            if (string.IsNullOrWhiteSpace(host))
                return new { error = "Host required" };

            string ip;
            try { ip = (await Dns.GetHostEntryAsync(host)).AddressList[0].ToString(); }
            catch { return new { error = $"Cannot resolve: {host}" }; }

            var ports = ParsePorts(portRange);
            if (ports.Count > 5000) return new { error = "Max 5000 ports" };

            var openPorts = new List<object>();
            var closed    = 0;

            var sem = new SemaphoreSlim(200, 200);
            var tasks = new List<Task>();

            foreach (var port in ports)
            {
                await sem.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using var tcp    = new TcpClient();
                        using var cts    = new CancellationTokenSource(500);
                        var connected = await tcp.ConnectAsync(ip, port).WaitAsync(TimeSpan.FromMilliseconds(500));
                        lock (openPorts)
                        {
                            WellKnown.TryGetValue(port, out var svc);
                            HighRisk.TryGetValue(port, out var risk);
                            if (risk == null) MedRisk.TryGetValue(port, out risk);
                            risk ??= "LOW";
                            openPorts.Add(new { port, service = svc ?? "Unknown", risk });
                        }
                    }
                    catch { Interlocked.Increment(ref closed); }
                    finally { sem.Release(); }
                }));
            }
            await Task.WhenAll(tasks);

            openPorts.Sort((a, b) =>
                ((int)a.GetType().GetProperty("port")!.GetValue(a)!)
                .CompareTo((int)b.GetType().GetProperty("port")!.GetValue(b)!));

            return new { host, ip, scanned = ports.Count, open = openPorts.Count, closed, ports = openPorts };
        }

        // ── Password Strength ──────────────────────────────────────────────────
        public object PasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password)) return new { error = "Password required" };

            bool hasLower   = false, hasUpper = false, hasDigit = false, hasSpecial = false;
            int  pool       = 0;
            foreach (char c in password)
            {
                if (char.IsLower(c))   hasLower   = true;
                if (char.IsUpper(c))   hasUpper   = true;
                if (char.IsDigit(c))   hasDigit   = true;
                if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }
            if (hasLower)   pool += 26;
            if (hasUpper)   pool += 26;
            if (hasDigit)   pool += 10;
            if (hasSpecial) pool += 32;

            double entropy = pool > 0 ? password.Length * Math.Log2(pool) : 0;
            int score = 0;
            if (password.Length >= 8)  score++;
            if (password.Length >= 12) score++;
            if (password.Length >= 16) score++;
            if (hasLower)   score++;
            if (hasUpper)   score++;
            if (hasDigit)   score++;
            if (hasSpecial) score++;
            if (entropy >= 50)  score++;
            if (entropy >= 70)  score++;
            if (entropy >= 100) score++;
            score = Math.Min(10, score);

            var issues = new List<string>();
            var tips   = new List<string>();
            if (!hasLower)   { issues.Add("No lowercase letters"); tips.Add("Add lowercase letters (a-z)"); }
            if (!hasUpper)   { issues.Add("No uppercase letters"); tips.Add("Add uppercase letters (A-Z)"); }
            if (!hasDigit)   { issues.Add("No numbers");           tips.Add("Add numbers (0-9)"); }
            if (!hasSpecial) { issues.Add("No special characters");tips.Add("Add symbols (!@#$%^&*)"); }
            if (password.Length < 8)  { issues.Add("Too short (< 8 chars)");   tips.Add("Use at least 12 characters"); }
            if (password.Length < 12) tips.Add("Aim for 16+ characters for best security");

            string level = score >= 9 ? "Very Strong" : score >= 7 ? "Strong"
                         : score >= 5 ? "Moderate"    : score >= 3 ? "Weak" : "Very Weak";
            string color = score >= 9 ? "low" : score >= 7 ? "low"
                         : score >= 5 ? "medium" : score >= 3 ? "high" : "critical";

            return new { score, level, color, entropy_bits = Math.Round(entropy, 1),
                         length = password.Length, hasLower, hasUpper, hasDigit, hasSpecial,
                         issues, tips };
        }

        // ── Hash Generator ─────────────────────────────────────────────────────
        public object GenerateHash(string text)
        {
            if (string.IsNullOrEmpty(text)) return new { error = "Text required" };
            var bytes = Encoding.UTF8.GetBytes(text);
            return new
            {
                input  = text,
                hashes = new Dictionary<string, string>
                {
                    ["md5"]    = Convert.ToHexString(MD5.HashData(bytes)).ToLower(),
                    ["sha1"]   = Convert.ToHexString(SHA1.HashData(bytes)).ToLower(),
                    ["sha256"] = Convert.ToHexString(SHA256.HashData(bytes)).ToLower(),
                    ["sha512"] = Convert.ToHexString(SHA512.HashData(bytes)).ToLower()
                }
            };
        }

        // ── SSL Checker ────────────────────────────────────────────────────────
        public async Task<object> CheckSslAsync(string host, int port = 443)
        {
            try
            {
                using var tcp = new TcpClient();
                await tcp.ConnectAsync(host, port).WaitAsync(TimeSpan.FromSeconds(10));

                using var ssl = new SslStream(tcp.GetStream(), false,
                    (_, _, _, _) => true);   // accept any cert for inspection
                await ssl.AuthenticateAsClientAsync(host);

                var cert = ssl.RemoteCertificate as X509Certificate2
                        ?? new X509Certificate2(ssl.RemoteCertificate!);

                var now         = DateTime.UtcNow;
                var notAfter    = cert.NotAfter.ToUniversalTime();
                var notBefore   = cert.NotBefore.ToUniversalTime();
                int daysLeft    = (int)(notAfter - now).TotalDays;

                var sans = new List<string>();
                foreach (var ext in cert.Extensions)
                {
                    if (ext.Oid?.FriendlyName == "Subject Alternative Name")
                    {
                        foreach (var line in ext.Format(false).Split(','))
                        {
                            var s = line.Trim().Replace("DNS Name=", "").Replace("DNS:", "");
                            if (!string.IsNullOrEmpty(s)) sans.Add(s);
                        }
                    }
                }

                return new
                {
                    valid         = true,
                    host,
                    subject_cn    = cert.GetNameInfo(X509NameType.SimpleName, false),
                    issuer_org    = cert.GetNameInfo(X509NameType.SimpleName, true),
                    not_before    = notBefore.ToString("yyyy-MM-dd"),
                    not_after     = notAfter.ToString("yyyy-MM-dd"),
                    days_remaining= daysLeft,
                    expired       = daysLeft < 0,
                    expiring_soon = daysLeft >= 0 && daysLeft < 30,
                    thumbprint    = cert.Thumbprint,
                    sans
                };
            }
            catch (Exception ex)
            {
                return new { valid = false, error = ex.Message };
            }
        }

        // ── Ping ───────────────────────────────────────────────────────────────
        public async Task<object> PingAsync(string host, int count = 4)
        {
            var times = new List<long>();
            int lost  = 0;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    using var ping  = new System.Net.NetworkInformation.Ping();
                    var reply = await ping.SendPingAsync(host, 2000);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        times.Add(reply.RoundtripTime);
                    else
                        lost++;
                }
                catch { lost++; }
                if (i < count - 1) await Task.Delay(200);
            }

            bool reachable = times.Count > 0;
            return new
            {
                host,
                reachable,
                sent      = count,
                received  = times.Count,
                lost,
                packet_loss_pct = (int)((double)lost / count * 100),
                min_ms    = reachable ? times.Min() : 0,
                max_ms    = reachable ? times.Max() : 0,
                avg_ms    = reachable ? (long)times.Average() : 0
            };
        }

        // ── Traceroute ─────────────────────────────────────────────────────────
        public async Task<object> TracerouteAsync(string host)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = "tracert",
                    Arguments              = $"-d -h 20 {host}",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };
                using var p = Process.Start(psi)!;
                var output = await p.StandardOutput.ReadToEndAsync()
                                    .WaitAsync(TimeSpan.FromSeconds(30));
                await p.WaitForExitAsync();
                return new { host, output };
            }
            catch (Exception ex) { return new { error = ex.Message }; }
        }

        // ── DNS Lookup ─────────────────────────────────────────────────────────
        public async Task<object> DnsLookupAsync(string domain)
        {
            var records = new Dictionary<string, object>();
            try
            {
                // A records
                var entry = await Dns.GetHostEntryAsync(domain);
                var aRecs = new List<string>();
                foreach (var ip in entry.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        aRecs.Add(ip.ToString());
                records["A"] = aRecs;

                // MX / NS / TXT via PowerShell
                var ps = PowerShellHelper.Run(
                    $"Resolve-DnsName -Name '{domain}' -Type MX -ErrorAction SilentlyContinue | " +
                    "Select-Object -ExpandProperty NameExchange");
                if (!string.IsNullOrEmpty(ps)) records["MX"] = ps.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var ns = PowerShellHelper.Run(
                    $"Resolve-DnsName -Name '{domain}' -Type NS -ErrorAction SilentlyContinue | " +
                    "Select-Object -ExpandProperty NameHost");
                if (!string.IsNullOrEmpty(ns)) records["NS"] = ns.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var txt = PowerShellHelper.Run(
                    $"Resolve-DnsName -Name '{domain}' -Type TXT -ErrorAction SilentlyContinue | " +
                    "Select-Object -ExpandProperty Strings");
                if (!string.IsNullOrEmpty(txt)) records["TXT"] = txt;

                return new { domain, records };
            }
            catch (Exception ex) { return new { domain, error = ex.Message, records }; }
        }

        // ── ARP Scan ───────────────────────────────────────────────────────────
        public async Task<object> ArpScanAsync() => await Task.Run(() =>
        {
            var hosts = new List<object>();
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = "arp",
                    Arguments              = "-a",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };
                using var p = Process.Start(psi)!;
                var out2 = p.StandardOutput.ReadToEnd();
                p.WaitForExit(5000);

                foreach (var line in out2.Split('\n'))
                {
                    var parts = line.Trim().Split(new[]{' ','\t'}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && IPAddress.TryParse(parts[0], out _))
                    {
                        var mac = parts[1];
                        if (mac is "ff-ff-ff-ff-ff-ff" or "---") continue;
                        hosts.Add(new
                        {
                            ip   = parts[0],
                            mac,
                            type = parts.Length > 2 ? parts[2] : "dynamic"
                        });
                    }
                }
            }
            catch (Exception ex) { return new { error = ex.Message, count = 0, hosts }; }
            return new { count = hosts.Count, hosts };
        });

        // ── Helpers ────────────────────────────────────────────────────────────
        private static List<int> ParsePorts(string range)
        {
            var ports = new List<int>();
            foreach (var part in range.Split(','))
            {
                var s = part.Trim();
                if (s.Contains('-'))
                {
                    var ab = s.Split('-');
                    if (int.TryParse(ab[0], out int a) && int.TryParse(ab[1], out int b))
                        for (int i = a; i <= b; i++) ports.Add(i);
                }
                else if (int.TryParse(s, out int p)) ports.Add(p);
            }
            return ports;
        }

        public void Dispose() { }
    }
}
