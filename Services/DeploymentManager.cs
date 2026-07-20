using ILAPEODeploy.Core;
using ILAPEODeploy.Models;

namespace ILAPEODeploy.Services;

public sealed class DeploymentManager
{
    private readonly Config _config;
    private readonly ComputerService _computerService;
    private readonly DomainService _domainService;
    private readonly NetworkService _networkService;
    private readonly InstallerService _installerService;

    public event EventHandler<string>? LogAdicionado;
    public event EventHandler<Software>? SoftwareStatusAlterado;
    public event EventHandler<int>? ProgressoAlterado;
    public event EventHandler<bool>? Concluido;

    public List<Software> Softwares { get; private set; } = new();

    public DeploymentManager()
    {
        _config = Config.Load();
        _computerService = new ComputerService();
        _domainService = new DomainService();
        _networkService = new NetworkService();
        _installerService = new InstallerService();
        InicializarSoftwares();
    }

    private void InicializarSoftwares()
    {
        Softwares = new List<Software>
        {
            new() { Tipo = TipoSoftware.Office,     Nome = "Microsoft Office",      Arquivo = _config.Office,     Obrigatorio = true },
            new() { Tipo = TipoSoftware.UltraVNC,    Nome = "UltraVNC",             Arquivo = _config.UltraVNC,   Parametros = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART", Obrigatorio = true },
            new() { Tipo = TipoSoftware.Chrome,     Nome = "Google Chrome",        Arquivo = _config.Chrome,     Obrigatorio = true },
            new() { Tipo = TipoSoftware.Firefox,    Nome = "Mozilla Firefox",      Arquivo = _config.Firefox,    Obrigatorio = true },
            new() { Tipo = TipoSoftware.Foxit,      Nome = "Foxit PDF Reader",     Arquivo = _config.Foxit,      Obrigatorio = true },
            new() { Tipo = TipoSoftware.WinRAR,     Nome = "WinRAR",               Arquivo = _config.WinRAR,     Obrigatorio = true },
            new() { Tipo = TipoSoftware.Ninite,     Nome = "Ninite Updater",       Arquivo = _config.Ninite,     Obrigatorio = false }
        };
    }

    public ComputerInfo ObterInfoComputador() => _computerService.GetComputerInfo();
    public bool VerificarAdministrador() => SystemInfoService.EhAdministrador();
    public bool VerificarDominio() => _domainService.EstaNoDominio();
    public string ObterDominio() => _domainService.ObterDominio();
    public bool VerificarServidor() => _networkService.ServidorDisponivel(_config.PastaInstaladores);
    public void AbrirConfiguracaoDominio() => _domainService.AbrirTelaDominio();

    public async Task PrepararComputadorAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            OnLogAdicionado("=======================================");
            OnLogAdicionado("  ILAPEO DEPLOY — INICIO DA PREPARACAO");
            OnLogAdicionado("=======================================");

            cancellationToken.ThrowIfCancellationRequested();

            // 1. Administrador
            if (!VerificarAdministrador())
            {
                OnLogAdicionado("ERRO: Execute o programa como Administrador.");
                OnConcluido(false);
                return;
            }
            OnLogAdicionado("OK Privilegios de Administrador confirmados.");

            // 2. Dominio
            if (!VerificarDominio())
            {
                OnLogAdicionado("Computador nao esta no dominio. Abrindo configuracoes do Windows...");
                AbrirConfiguracaoDominio();
                OnConcluido(false);
                return;
            }
            OnLogAdicionado($"OK Dominio confirmado: {ObterDominio()}");

            // 3. Servidor
            if (!VerificarServidor())
            {
                OnLogAdicionado($"ERRO Servidor de instaladores inacessivel: {_config.PastaInstaladores}");
                OnConcluido(false);
                return;
            }
            OnLogAdicionado("OK Servidor de instaladores acessivel.");

            // 4. Softwares
            var obrigatorios = Softwares.Where(s => s.Obrigatorio).ToList();
            int total = obrigatorios.Count;
            int atual = 0;
            OnProgressoAlterado(0);

            foreach (var software in obrigatorios)
            {
                cancellationToken.ThrowIfCancellationRequested();

                software.Status = StatusInstalacao.Verificando;
                OnSoftwareStatusAlterado(software);
                OnLogAdicionado($"Verificando {software.Nome}...");

                if (DetectionService.EstaInstalado(software))
                {
                    software.Instalado = true;
                    software.Status = StatusInstalacao.Concluido;
                    OnLogAdicionado($"  OK {software.Nome} ja esta instalado.");
                }
                else
                {
                    // Tenta instalar com retry (max 2 tentativas)
                    bool instalado = await TentarInstalarComRetryAsync(software, cancellationToken);

                    if (instalado)
                    {
                        software.Instalado = true;
                        software.Status = StatusInstalacao.Concluido;
                    }
                    else
                    {
                        software.Status = StatusInstalacao.Erro;
                    }
                }

                OnSoftwareStatusAlterado(software);
                atual++;
                OnProgressoAlterado((int)((double)atual / total * 100));
            }

            // 5. UltraVNC Admin
            var uvnc = Softwares.FirstOrDefault(s => s.Tipo == TipoSoftware.UltraVNC && s.Instalado);
            if (uvnc != null)
            {
                OnLogAdicionado("Abrindo UltraVNC Admin Properties...");
                AbrirUltraVNCAdmin();
            }

            OnLogAdicionado("=======================================");
            OnLogAdicionado("  PREPARACAO CONCLUIDA");
            OnLogAdicionado("=======================================");
            OnConcluido(true);
        }
        catch (OperationCanceledException)
        {
            OnLogAdicionado("OPERACAO CANCELADA pelo usuario.");
            Logger.Log("[CANCELADO] Preparacao interrompida pelo usuario.");
            OnConcluido(false);
        }
        catch (Exception ex)
        {
            OnLogAdicionado($"ERRO CRITICO: {ex.Message}");
            Logger.Log($"[CRITICO] DeploymentManager: {ex}");
            OnConcluido(false);
        }
    }

    private async Task<bool> TentarInstalarComRetryAsync(Software software, CancellationToken cancellationToken)
    {
        const int maxTentativas = 2;

        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            software.Status = StatusInstalacao.Instalando;
            OnSoftwareStatusAlterado(software);

            if (tentativa > 1)
            {
                OnLogAdicionado($"  Tentativa {tentativa} de {maxTentativas} para {software.Nome}...");
            }
            else
            {
                OnLogAdicionado($"  Instalando {software.Nome}...");
            }

            var resultado = await _installerService.InstalarAsync(software, _config.PastaInstaladores);

            if (resultado.Sucesso)
            {
                // VERIFICACAO POS-INSTALACAO: confirma que realmente esta instalado
                OnLogAdicionado($"  Verificando instalacao de {software.Nome}...");
                await Task.Delay(500, cancellationToken); // pequena pausa para o sistema registrar

                if (DetectionService.EstaInstalado(software))
                {
                    OnLogAdicionado($"  OK {software.Nome} instalado e verificado. ({resultado.Tempo.TotalSeconds:F1}s)");
                    return true;
                }
                else
                {
                    OnLogAdicionado($"  AVISO {software.Nome} instalador retornou sucesso, mas software nao detectado.");
                    Logger.Log($"[AVISO] Pos-instalacao: {software.Nome} nao detectado apos instalacao com ExitCode 0.");

                    if (tentativa < maxTentativas)
                    {
                        OnLogAdicionado($"  Aguardando 3s antes da proxima tentativa...");
                        await Task.Delay(3000, cancellationToken);
                        continue;
                    }
                }
            }
            else
            {
                OnLogAdicionado($"  ERRO {resultado.Mensagem} ({resultado.Tempo.TotalSeconds:F1}s)");

                if (tentativa < maxTentativas)
                {
                    OnLogAdicionado($"  Aguardando 3s antes da proxima tentativa...");
                    await Task.Delay(3000, cancellationToken);
                    continue;
                }
            }
        }

        OnLogAdicionado($"  FALHA {software.Nome} nao foi instalado apos {maxTentativas} tentativas.");
        return false;
    }

    private void AbrirUltraVNCAdmin()
    {
        string[] caminhos =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"UltraVNC\uvnc_settings.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"UltraVNC\vncproperties.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"UltraVNC\uvnc_settings.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"UltraVNC\vncproperties.exe"),
            @"C:\Program Files\uvnc bvba\UltraVNC\uvnc_settings.exe",
            @"C:\Program Files\uvnc bvba\UltraVNC\vncproperties.exe",
            @"C:\Program Files (x86)\uvnc bvba\UltraVNC\uvnc_settings.exe",
            @"C:\Program Files (x86)\uvnc bvba\UltraVNC\vncproperties.exe"
        };

        foreach (var caminho in caminhos)
        {
            if (!File.Exists(caminho)) continue;
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = caminho,
                    UseShellExecute = true
                });
                OnLogAdicionado("  OK UltraVNC Admin Properties aberto.");
                return;
            }
            catch (Exception ex)
            {
                OnLogAdicionado($"  ERRO ao abrir UltraVNC: {ex.Message}");
            }
        }
        OnLogAdicionado("  ! UltraVNC Admin Properties nao encontrado.");
    }

    private void OnLogAdicionado(string mensagem) => LogAdicionado?.Invoke(this, mensagem);
    private void OnSoftwareStatusAlterado(Software software) => SoftwareStatusAlterado?.Invoke(this, software);
    private void OnProgressoAlterado(int percentual) => ProgressoAlterado?.Invoke(this, percentual);
    private void OnConcluido(bool sucesso) => Concluido?.Invoke(this, sucesso);
}