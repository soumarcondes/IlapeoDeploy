using System.Text.Json;

namespace ILAPEODeploy.Core;

public sealed class Config
{
    public string PastaInstaladores { get; set; } = "";
    public string Office { get; set; } = "";
    public string UltraVNC { get; set; } = "";
    public string Ninite { get; set; } = "";
    public string Chrome { get; set; } = "";
    public string Firefox { get; set; } = "";
    public string Foxit { get; set; } = "";
    public string WinRAR { get; set; } = "";

    public static Config Load()
    {
        string caminho = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json");

        if (!File.Exists(caminho))
            throw new FileNotFoundException($"Arquivo de configuracao nao encontrado: {caminho}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(caminho), options);
        if (config == null)
            throw new InvalidOperationException("Falha ao desserializar config.json");

        return config;
    }
}
