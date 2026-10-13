using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ZenithSecurity.Security
{
    public class NetworkScanner
    {
        public async Task<object> GetAdaptersAsync() => await Task.Run(() =>
        {
            var adapters = new List<object>();
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    var ipProps = nic.GetIPProperties();
                    var ips     = new List<string>();
                    foreach (var ua in ipProps.UnicastAddresses)
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork ||
                            ua.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            ips.Add(ua.Address.ToString());

                    adapters.Add(new
                    {
                        name        = nic.Name,
                        description = nic.Description,
                        type        = nic.NetworkInterfaceType.ToString(),
                        status      = nic.OperationalStatus.ToString(),
                        speed       = nic.Speed,
                        mac         = nic.GetPhysicalAddress().ToString(),
                        ips,
                        gateway     = ipProps.GatewayAddresses.Count > 0
                                      ? ipProps.GatewayAddresses[0].Address.ToString() : "",
                        dns         = ipProps.DnsAddresses.Count > 0
                                      ? ipProps.DnsAddresses[0].ToString() : ""
                    });
                }
                catch { }
            }
            return new { count = adapters.Count, adapters };
        });

        public async Task<object> GetConnectionsAsync() => await Task.Run(() =>
        {
            var conns = new List<object>();
            try
            {
                var props = IPGlobalProperties.GetIPGlobalProperties();

                foreach (var ep in props.GetActiveTcpListeners())
                    conns.Add(new { proto="TCP", local=ep.ToString(), remote="", state="LISTEN" });

                foreach (var conn in props.GetActiveTcpConnections())
                    conns.Add(new
                    {
                        proto  = "TCP",
                        local  = conn.LocalEndPoint.ToString(),
                        remote = conn.RemoteEndPoint.ToString(),
                        state  = conn.State.ToString()
                    });
            }
            catch (Exception ex)
            {
                return new { error = ex.Message, connections = conns };
            }
            return new { count = conns.Count, connections = conns };
        });

        public async Task<object> GetStatsAsync() => await Task.Run(() =>
        {
            try
            {
                var props = IPGlobalProperties.GetIPGlobalProperties();
                var tcp   = props.GetTcpIPv4Statistics();
                var ip    = props.GetIPv4GlobalStatistics();
                return new
                {
                    tcpActive      = tcp.CurrentConnections,
                    tcpInFailed    = tcp.FailedConnectionAttempts,
                    packetsReceived= ip.ReceivedPackets,
                    packetsSent    = ip.OutputPacketsWithNoRoute,
                    errors         = ip.ReceivedPacketsWithErrors
                };
            }
            catch (Exception ex) { return new { error = ex.Message }; }
        });
    }
}
