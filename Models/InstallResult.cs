namespace ILAPEODeploy.Models;

public sealed class InstallResult
{
    public string Software { get; set; } = "";
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = "";
    public TimeSpan Tempo { get; set; }
}
