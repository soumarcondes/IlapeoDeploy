using System.Diagnostics;
using System.Management;

namespace ILAPEODeploy.Services;

public sealed class DomainService
{
    public bool EstaNoDominio()
    {
        try
        {
            using var search = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject item in search.Get())
            {
                return Convert.ToBoolean(item["PartOfDomain"]);
            }
        }
        catch { }
        return false;
    }

    public string ObterDominio()
    {
        try
        {
            using var search = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject item in search.Get())
            {
                return item["Domain"]?.ToString()?.Trim() ?? "";
            }
        }
        catch { }
        return "";
    }

    public bool EstaNoDominioEsperado(string dominioEsperado)
    {
        string atual = ObterDominio();
        return !string.IsNullOrWhiteSpace(atual) &&
               atual.Equals(dominioEsperado, StringComparison.OrdinalIgnoreCase);
    }

    public void AbrirTelaDominio()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "sysdm.cpl",
            UseShellExecute = true
        });
    }
}
