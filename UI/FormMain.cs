using ILAPEODeploy.Core;
using ILAPEODeploy.Models;
using ILAPEODeploy.Services;

namespace ILAPEODeploy.UI;

public partial class FormMain : Form
{
    private readonly DeploymentManager _deploymentManager;
    private readonly Dictionary<string, Panel> _cards = new();
    private readonly Dictionary<string, Label> _cardLabels = new();
    private readonly Dictionary<TipoSoftware, Panel> _softwarePanels = new();
    private CancellationTokenSource? _cts;

    public FormMain()
    {
        InitializeComponent();
        _deploymentManager = new DeploymentManager();
        ConfigurarEventos();
        CarregarInformacoesIniciais();
    }

    private void ConfigurarEventos()
    {
        _deploymentManager.LogAdicionado += (s, msg) =>
        {
            if (InvokeRequired) Invoke(() => AdicionarLogColorido(msg));
            else AdicionarLogColorido(msg);
        };

        _deploymentManager.ProgressoAlterado += (s, pct) =>
        {
            if (InvokeRequired) Invoke(() => AtualizarProgresso(pct));
            else AtualizarProgresso(pct);
        };

        _deploymentManager.Concluido += (s, ok) =>
        {
            if (InvokeRequired) Invoke(() => FinalizarProcesso(ok));
            else FinalizarProcesso(ok);
        };

        _deploymentManager.SoftwareStatusAlterado += (s, sw) =>
        {
            if (InvokeRequired) Invoke(() => AtualizarStatusSoftware(sw));
            else AtualizarStatusSoftware(sw);
        };
    }

    private void FormMain_Load(object? sender, EventArgs e)
    {
        AtualizarCard("admin", _deploymentManager.VerificarAdministrador());
        AtualizarCard("dominio", _deploymentManager.VerificarDominio());
        AtualizarCard("servidor", _deploymentManager.VerificarServidor());
    }

    private void CarregarInformacoesIniciais()
    {
        try
        {
            var info = _deploymentManager.ObterInfoComputador();

            lblComputadorValor.Text = info.ComputerName;
            lblUsuarioValor.Text = info.UserName;
            lblWindowsValor.Text = info.WindowsVersion;
            lblDominioValor.Text = info.Domain;
            lblIPValor.Text = info.IPv4;
            lblMACValor.Text = info.MacAddress;
            lblProcessadorValor.Text = info.Processor;
            lblRAMValor.Text = info.Ram;
            lblDiscoValor.Text = info.Disk;
            lblSerialValor.Text = info.SerialNumber;

            AdicionarLogColorido($"Sistema iniciado: {info.ComputerName} | {info.WindowsVersion}");

            // Aviso se o nome parece temporario/padrao
            if (info.ComputerName.Contains("DESKTOP", StringComparison.OrdinalIgnoreCase) ||
                info.ComputerName.Contains("LAPTOP", StringComparison.OrdinalIgnoreCase) ||
                info.ComputerName.Contains("WIN-", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarLogColorido("AVISO: Nome do computador parece temporario. Use o botao 'Renomear' se necessario.");
            }
        }
        catch (Exception ex)
        {
            AdicionarLogColorido($"Erro ao carregar informacoes: {ex.Message}");
        }
    }

    private void AtualizarCard(string chave, bool ok)
    {
        if (!_cards.ContainsKey(chave)) return;

        var card = _cards[chave];
        var lbl = _cardLabels[chave];

        card.BackColor = ok ? Color.FromArgb(35, 134, 54) : Color.FromArgb(218, 54, 51);
        lbl.ForeColor = Color.White;
        lbl.Text = ok ? "OK  " + (lbl.Tag?.ToString() ?? "") : "ERRO  " + (lbl.Tag?.ToString() ?? "");
    }

    private void AdicionarLogColorido(string mensagem)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");

        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;

        // Cor do timestamp (cinza)
        txtLog.SelectionColor = Color.FromArgb(139, 148, 158);
        txtLog.AppendText($"[{timestamp}] ");

        // Cor da mensagem baseada no conteudo
        Color corMensagem = Color.FromArgb(201, 209, 217); // padrao cinza claro

        string msgLower = mensagem.ToLowerInvariant();

        if (msgLower.Contains("erro") || msgLower.Contains("falha") || msgLower.Contains("critico"))
            corMensagem = Color.FromArgb(255, 123, 114); // vermelho claro
        else if (msgLower.Contains("ok") || msgLower.Contains("sucesso") || msgLower.Contains("concluida"))
            corMensagem = Color.FromArgb(127, 255, 127); // verde claro
        else if (msgLower.Contains("aviso") || msgLower.Contains("warning"))
            corMensagem = Color.FromArgb(255, 215, 100); // amarelo
        else if (msgLower.Contains("instalando") || msgLower.Contains("verificando") || msgLower.Contains("iniciando"))
            corMensagem = Color.FromArgb(120, 180, 255); // azul claro
        else if (msgLower.Contains("cancelado"))
            corMensagem = Color.FromArgb(255, 160, 80); // laranja

        txtLog.SelectionColor = corMensagem;
        txtLog.AppendText(mensagem);
        txtLog.AppendText(Environment.NewLine);

        // Reset para cor padrao
        txtLog.SelectionColor = Color.FromArgb(201, 209, 217);
        txtLog.ScrollToCaret();
    }

    private void AtualizarProgresso(int percentual)
    {
        progressBar.Value = Math.Min(100, Math.Max(0, percentual));
        lblProgresso.Text = $"{percentual}%";
    }

    private void AtualizarStatusSoftware(Software sw)
    {
        if (!_softwarePanels.ContainsKey(sw.Tipo)) return;

        var panel = _softwarePanels[sw.Tipo];
        var cor = sw.Status switch
        {
            StatusInstalacao.Concluido => Color.FromArgb(35, 134, 54),
            StatusInstalacao.Erro => Color.FromArgb(218, 54, 51),
            StatusInstalacao.Instalando => Color.FromArgb(31, 111, 235),
            StatusInstalacao.Verificando => Color.FromArgb(210, 153, 29),
            _ => Color.FromArgb(48, 54, 61)
        };

        panel.BackColor = cor;
    }

    private void FinalizarProcesso(bool sucesso)
    {
        btnPreparar.Enabled = true;
        btnPreparar.Text = "Preparar Computador";
        btnPreparar.BackColor = Color.FromArgb(35, 134, 54);

        _cts?.Dispose();
        _cts = null;

        if (sucesso)
        {
            AdicionarLogColorido("Processo concluido com sucesso.");
            MessageBox.Show("Preparacao do computador concluida com sucesso!",
                "ILAPEO Deploy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            AdicionarLogColorido("Processo finalizado com pendencias.");
        }
    }

    private async void btnPreparar_Click(object? sender, EventArgs e)
    {
        if (!_deploymentManager.VerificarAdministrador())
        {
            MessageBox.Show("Execute o programa como Administrador.",
                "ILAPEO Deploy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Se ja esta processando, interpreta como CANCELAR
        if (_cts != null)
        {
            var result = MessageBox.Show("Deseja cancelar a operacao atual?",
                "ILAPEO Deploy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _cts.Cancel();
                AdicionarLogColorido("Cancelamento solicitado pelo usuario...");
            }
            return;
        }

        _cts = new CancellationTokenSource();

        btnPreparar.Text = "Cancelar";
        btnPreparar.BackColor = Color.FromArgb(218, 54, 51);
        txtLog.Clear();

        foreach (var sw in _deploymentManager.Softwares)
            AtualizarStatusSoftware(sw);

        await _deploymentManager.PrepararComputadorAsync(_cts.Token);
    }

    private void btnDominio_Click(object? sender, EventArgs e)
    {
        _deploymentManager.AbrirConfiguracaoDominio();
    }

    private void btnSair_Click(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        Application.Exit();
    }
}