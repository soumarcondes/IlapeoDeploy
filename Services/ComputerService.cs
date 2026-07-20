using ILAPEODeploy.Core;
using ILAPEODeploy.Models;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.DirectoryServices;

namespace ILAPEODeploy.Services;

public sealed class ComputerService
{
    public ComputerInfo GetComputerInfo()
    {
        var info = new ComputerInfo
        {
            ComputerName = Environment.MachineName,
            UserName = Environment.UserName,
            Domain = Environment.UserDomainName,
            WindowsVersion = $"{Environment.OSVersion.VersionString} (Build {Environment.OSVersion.Version.Build})"
        };

        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject item in searcher.Get())
            {
                info.Manufacturer = item["Manufacturer"]?.ToString()?.Trim() ?? "";
                info.Model = item["Model"]?.ToString()?.Trim() ?? "";

                if (item["TotalPhysicalMemory"] != null &&
                    ulong.TryParse(item["TotalPhysicalMemory"]?.ToString(), out ulong mem))
                {
                    info.Ram = $"{mem / (1024UL * 1024 * 1024)} GB";
                }
            }
        }
        catch { }

        try
        {
            using var bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (ManagementObject item in bios.Get())
            {
                info.SerialNumber = item["SerialNumber"]?.ToString()?.Trim() ?? "";
            }
        }
        catch { }

        try
        {
            using var cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject item in cpu.Get())
            {
                info.Processor = item["Name"]?.ToString()?.Trim() ?? "";
            }
        }
        catch { }

        try
        {
            using var disk = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DeviceID='C:'");
            foreach (ManagementObject item in disk.Get())
            {
                if (item["Size"] != null && ulong.TryParse(item["Size"]?.ToString(), out ulong size))
                {
                    string livre = item["FreeSpace"] != null && ulong.TryParse(item["FreeSpace"]?.ToString(), out ulong free)
                        ? $"{free / (1024UL * 1024 * 1024)} GB livre"
                        : "";
                    info.Disk = $"{size / (1024UL * 1024 * 1024)} GB total ({livre})";
                }
            }
        }
        catch { }

        try
        {
            info.IPv4 = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                .ToString() ?? "";
        }
        catch { }

        try
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            if (nic != null)
            {
                info.MacAddress = string.Join(":",
                    nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
            }
        }
        catch { }

        return info;
    }

    /// <summary>
    /// Renomeia o computador e agenda reinicio.
    /// </summary>
    public static bool RenomearComputador(string novoNome, out string mensagem)
    {
        try
        {
            // Valida o nome
            if (string.IsNullOrWhiteSpace(novoNome) || novoNome.Length > 15)
            {
                mensagem = "Nome invalido. Maximo 15 caracteres.";
                return false;
            }

            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject item in searcher.Get())
            {
                var result = item.InvokeMethod("Rename", new object[] { novoNome, string.Empty, string.Empty });
                int codigo = Convert.ToInt32(result);

                if (codigo == 0)
                {
                    mensagem = $"Computador renomeado para '{novoNome}'. Reinicio necessario.";
                    Logger.Log($"[RENAME] Computador renomeado para {novoNome}");
                    return true;
                }
                else
                {
                    mensagem = $"Falha ao renomear. Codigo WMI: {codigo}";
                    Logger.Log($"[ERRO] Rename falhou com codigo {codigo}");
                    return false;
                }
            }

            mensagem = "Nao foi possivel acessar WMI.";
            return false;
        }
        catch (Exception ex)
        {
            mensagem = $"Erro: {ex.Message}";
            Logger.Log($"[EXCECAO] RenomearComputador: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Busca o proximo numero disponivel para o perfil na rede (Active Directory / DNS).
    /// Se nao conseguir consultar AD, retorna o primeiro numero livre local.
    /// </summary>
    public static int ObterProximoNumeroDisponivel(PerfilComputador perfil)
    {
        try
        {
            // Tenta consultar Active Directory para ver quais ja existem
            var existentes = new HashSet<int>();
            string dominio = Environment.UserDomainName;

            if (!string.IsNullOrWhiteSpace(dominio))
            {
                try
                {
                    using var entry = new DirectoryEntry($"LDAP://{dominio}");
                    using var searcher = new DirectorySearcher(entry)
                    {
                        Filter = $"(&(objectCategory=computer)(name={perfil.Prefixo}*))",
                        PropertiesToLoad = { "name" }
                    };

                    var results = searcher.FindAll();
                    foreach (SearchResult result in results)
                    {
                        string? nome = result.Properties["name"]?[0]?.ToString();
                        if (nome != null && nome.StartsWith(perfil.Prefixo))
                        {
                            string numeroStr = nome[perfil.Prefixo.Length..];
                            if (int.TryParse(numeroStr, out int num))
                                existentes.Add(num);
                        }
                    }
                }
                catch
                {
                    // Se AD nao disponivel, continua com logica local
                }
            }

            // Retorna o primeiro numero nao usado
            for (int i = perfil.NumeroInicial; i <= perfil.NumeroMaximo; i++)
            {
                if (!existentes.Contains(i))
                    return i;
            }

            return perfil.NumeroInicial; // Fallback
        }
        catch
        {
            return perfil.NumeroInicial;
        }
    }
}