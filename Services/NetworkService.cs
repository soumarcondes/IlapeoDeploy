using System.Net;
using System.Net.NetworkInformation;

namespace ILAPEODeploy.Services;

public sealed class NetworkService
{
    public string GetIPv4()
    {
        try
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                .ToString() ?? "";
        }
        catch { return ""; }
    }

    public string GetMac()
    {
        try
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            return nic != null
                ? string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")))
                : "";
        }
        catch { return ""; }
    }

    public bool ServidorDisponivel(string caminho)
    {
        try { return Directory.Exists(caminho); }
        catch { return false; }
    }

    public bool PingHost(string host, int timeout = 1000)
    {
        try
        {
            using var ping = new Ping();
            var reply = ping.Send(host, timeout);
            return reply?.Status == IPStatus.Success;
        }
        catch { return false; }
    }
}
