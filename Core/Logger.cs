using System.Text;

namespace ILAPEODeploy.Core;

public static class Logger
{
    private static readonly object LockObject = new();

    public static void Log(string texto)
    {
        try
        {
            Directory.CreateDirectory(Constants.PastaLog);

            string arquivo = Path.Combine(
                Constants.PastaLog,
                DateTime.Now.ToString("yyyyMMdd") + ".log");

            lock (LockObject)
            {
                File.AppendAllText(
                    arquivo,
                    $"[{DateTime.Now:HH:mm:ss}] {texto}{Environment.NewLine}",
                    Encoding.UTF8);
            }
        }
        catch
        {
            // Silencioso
        }
    }
}
