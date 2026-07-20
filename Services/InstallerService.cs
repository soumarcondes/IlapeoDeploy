using System.Diagnostics;
using ILAPEODeploy.Core;
using ILAPEODeploy.Models;

namespace ILAPEODeploy.Services;

public sealed class InstallerService
{
    private static readonly TimeSpan TimeoutPadrao = TimeSpan.FromMinutes(10);

    public async Task<InstallResult> InstalarAsync(Software software, string pastaInstaladores, CancellationToken cancellationToken = default)
    {
        var resultado = new InstallResult { Software = software.Nome };
        var tempo = Stopwatch.StartNew();

        try
        {
            string caminho = Path.Combine(pastaInstaladores, software.Arquivo);

            if (!File.Exists(caminho))
            {
                tempo.Stop();
                resultado.Sucesso = false;
                resultado.Mensagem = $"Arquivo nao encontrado: {software.Arquivo}";
                resultado.Tempo = tempo.Elapsed;
                Logger.Log($"[ERRO] {resultado.Mensagem}");
                return resultado;
            }

            Logger.Log($"[INSTALACAO] Iniciando {software.Nome}...");

            var psi = new ProcessStartInfo
            {
                FileName = caminho,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = pastaInstaladores
            };

            if (!string.IsNullOrWhiteSpace(software.Parametros))
                psi.Arguments = software.Parametros;

            using var processo = Process.Start(psi);
            if (processo == null)
            {
                tempo.Stop();
                resultado.Sucesso = false;
                resultado.Mensagem = "Nao foi possivel iniciar o processo de instalacao.";
                resultado.Tempo = tempo.Elapsed;
                Logger.Log($"[ERRO] {software.Nome}: {resultado.Mensagem}");
                return resultado;
            }

            // Aguarda com timeout e suporte a cancelamento
            using var timeoutCts = new CancellationTokenSource(TimeoutPadrao);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                await processo.WaitForExitAsync(linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
                if (timeoutCts.IsCancellationRequested)
                {
                    try { processo.Kill(); } catch { }
                    tempo.Stop();
                    resultado.Sucesso = false;
                    resultado.Mensagem = "Tempo limite excedido (10 minutos).";
                    resultado.Tempo = tempo.Elapsed;
                    Logger.Log($"[TIMEOUT] {software.Nome}: instalacao cancelada por timeout.");
                    return resultado;
                }
                throw; // Re-lanca se foi cancelamento do usuario
            }

            tempo.Stop();

            resultado.Tempo = tempo.Elapsed;
            resultado.Sucesso = processo.ExitCode == 0;
            resultado.Mensagem = processo.ExitCode == 0
                ? $"{software.Nome} instalado com sucesso."
                : $"{software.Nome} terminou com codigo {processo.ExitCode}.";

            Logger.Log(resultado.Sucesso
                ? $"[OK] {resultado.Mensagem}"
                : $"[AVISO] {resultado.Mensagem}");

            return resultado;
        }
        catch (Exception ex)
        {
            tempo.Stop();
            resultado.Tempo = tempo.Elapsed;
            resultado.Sucesso = false;
            resultado.Mensagem = ex.Message;
            Logger.Log($"[EXCECAO] {software.Nome}: {ex}");
            return resultado;
        }
    }
}