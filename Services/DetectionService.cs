using ILAPEODeploy.Models;
using Microsoft.Win32;

namespace ILAPEODeploy.Services;

public static class DetectionService
{
    public static bool EstaInstalado(Software software)
    {
        return software.Tipo switch
        {
            TipoSoftware.Office => OfficeInstalado(),
            TipoSoftware.UltraVNC => UltraVNCInstalado(),
            TipoSoftware.Chrome => ChromeInstalado(),
            TipoSoftware.Firefox => FirefoxInstalado(),
            TipoSoftware.Foxit => FoxitInstalado(),
            TipoSoftware.WinRAR => WinRARInstalado(),
            TipoSoftware.Ninite => false,
            _ => false
        };
    }

    private static bool OfficeInstalado()
    {
        return ProgramaInstalado("Microsoft 365") ||
               ProgramaInstalado("Microsoft Office") ||
               File.Exists(@"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE") ||
               File.Exists(@"C:\Program Files (x86)\Microsoft Office\root\Office16\WINWORD.EXE");
    }

    private static bool UltraVNCInstalado()
    {
        return ProgramaInstalado("UltraVNC") ||
               File.Exists(@"C:\Program Files\UltraVNC\winvnc.exe") ||
               File.Exists(@"C:\Program Files\uvnc bvba\UltraVNC\winvnc.exe") ||
               File.Exists(@"C:\Program Files (x86)\UltraVNC\winvnc.exe") ||
               File.Exists(@"C:\Program Files (x86)\uvnc bvba\UltraVNC\winvnc.exe");
    }

    private static bool ChromeInstalado()
    {
        return ProgramaInstalado("Google Chrome") ||
               File.Exists(@"C:\Program Files\Google\Chrome\Application\chrome.exe") ||
               File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
    }

    private static bool FirefoxInstalado()
    {
        return ProgramaInstalado("Mozilla Firefox") ||
               File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe") ||
               File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe");
    }

    private static bool WinRARInstalado()
    {
        return ProgramaInstalado("WinRAR") ||
               File.Exists(@"C:\Program Files\WinRAR\WinRAR.exe") ||
               File.Exists(@"C:\Program Files (x86)\WinRAR\WinRAR.exe");
    }

    private static bool FoxitInstalado()
    {
        return ProgramaInstalado("Foxit PDF Reader") ||
               ProgramaInstalado("Foxit Reader") ||
               File.Exists(@"C:\Program Files\Foxit Software\Foxit PDF Reader\FoxitPDFReader.exe") ||
               File.Exists(@"C:\Program Files (x86)\Foxit Software\Foxit PDF Reader\FoxitPDFReader.exe");
    }

    private static bool ProgramaInstalado(string nome)
    {
        string[] chaves =
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (string chave in chaves)
        {
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(chave);
                if (key == null) continue;

                foreach (string sub in key.GetSubKeyNames())
                {
                    try
                    {
                        using RegistryKey? programa = key.OpenSubKey(sub);
                        string? displayName = programa?.GetValue("DisplayName") as string;

                        if (!string.IsNullOrWhiteSpace(displayName) &&
                            displayName.Contains(nome, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        return false;
    }
}
