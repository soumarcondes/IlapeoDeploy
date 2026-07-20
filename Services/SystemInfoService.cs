using System.Security.Principal;

namespace ILAPEODeploy.Services;

public static class SystemInfoService
{
    public static bool EhAdministrador()
    {
        try
        {
            var identidade = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identidade);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }
}
