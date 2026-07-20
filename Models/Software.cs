namespace ILAPEODeploy.Models;

public sealed class Software
{
    public TipoSoftware Tipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Arquivo { get; set; } = string.Empty;
    public string? Parametros { get; set; }
    public bool Instalado { get; set; }
    public bool Obrigatorio { get; set; } = true;
    public StatusInstalacao Status { get; set; } = StatusInstalacao.Pendente;
}
