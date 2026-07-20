namespace ILAPEODeploy.Models;

public sealed class PerfilComputador
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Prefixo { get; set; } = string.Empty;
    public int NumeroInicial { get; set; } = 1;
    public int NumeroMaximo { get; set; } = 999;
    public int Digitos { get; set; } = 3;
    public string Descricao { get; set; } = string.Empty;
    public string Icone { get; set; } = "💻";

    public string FormatarNumero(int numero)
    {
        return numero.ToString().PadLeft(Digitos, '0');
    }

    public string GerarNome(int numero)
    {
        return $"{Prefixo}{FormatarNumero(numero)}";
    }
}
