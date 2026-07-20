#nullable disable
using ILAPEODeploy.Models;
using ILAPEODeploy.Core;

namespace ILAPEODeploy.UI;

partial class FormMain
{
    private System.ComponentModel.IContainer components = null;

    private Panel panelLateral;
    private Panel panelTopo;
    private Panel panelConteudo;
    private Panel panelLog;
    private Panel panelStatus;
    private Panel panelSoftwares;

    private Label lblTitulo;
    private Label lblComputador;
    private Label lblComputadorValor;
    private Label lblUsuario;
    private Label lblUsuarioValor;
    private Label lblWindows;
    private Label lblWindowsValor;
    private Label lblDominio;
    private Label lblDominioValor;
    private Label lblIP;
    private Label lblIPValor;
    private Label lblMAC;
    private Label lblMACValor;
    private Label lblProcessador;
    private Label lblProcessadorValor;
    private Label lblRAM;
    private Label lblRAMValor;
    private Label lblDisco;
    private Label lblDiscoValor;
    private Label lblSerial;
    private Label lblSerialValor;

    private Button btnPreparar;
    private Button btnDominio;
    private Button btnSair;

    private ProgressBar progressBar;
    private Label lblProgresso;

    private RichTextBox txtLog;

    private Panel cardAdmin;
    private Panel cardDominio;
    private Panel cardServidor;
    private Label lblCardAdmin;
    private Label lblCardDominio;
    private Label lblCardServidor;

    private Panel swOffice;
    private Panel swUltraVNC;
    private Panel swChrome;
    private Panel swFirefox;
    private Panel swFoxit;
    private Panel swWinRAR;
    private Panel swNinite;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        Color corFundo = Color.FromArgb(13, 17, 23);
        Color corCard = Color.FromArgb(22, 27, 34);
        Color corBorda = Color.FromArgb(48, 54, 61);
        Color corTexto = Color.FromArgb(201, 209, 217);
        Color corDestaque = Color.FromArgb(31, 111, 235);
        Color corBotao = Color.FromArgb(35, 134, 54);

        SuspendLayout();
        BackColor = corFundo;
        ForeColor = corTexto;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ClientSize = new Size(1200, 800);
        MinimumSize = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Text = VersionInfo.Titulo;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;

        panelLateral = new Panel
        {
            Dock = DockStyle.Left,
            Width = 260,
            BackColor = corCard,
            Padding = new Padding(20)
        };

        var lblLogo = new Label
        {
            Text = "ILAPEO",
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 20)
        };

        var lblDeploy = new Label
        {
            Text = "DEPLOY",
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = corDestaque,
            AutoSize = true,
            Location = new Point(20, 55)
        };

        var lblVersao = new Label
        {
            Text = $"v{VersionInfo.Versao}",
            Font = new Font("Segoe UI", 8F),
            ForeColor = Color.FromArgb(139, 148, 158),
            AutoSize = true,
            Location = new Point(20, 82)
        };

        var sep1 = new Panel
        {
            Height = 1,
            BackColor = corBorda,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 100, 0, 20)
        };

        btnPreparar = new Button
        {
            Text = "Preparar Computador",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = corBotao,
            FlatStyle = FlatStyle.Flat,
            Height = 48,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 20, 0, 10),
            Cursor = Cursors.Hand
        };
        btnPreparar.FlatAppearance.BorderSize = 0;
        btnPreparar.Click += btnPreparar_Click;

        btnDominio = new Button
        {
            Text = "Configurar Dominio",
            Font = new Font("Segoe UI", 10F),
            ForeColor = corTexto,
            BackColor = corCard,
            FlatStyle = FlatStyle.Flat,
            Height = 40,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 5),
            Cursor = Cursors.Hand
        };
        btnDominio.FlatAppearance.BorderColor = corBorda;
        btnDominio.FlatAppearance.BorderSize = 1;
        btnDominio.Click += btnDominio_Click;

        btnSair = new Button
        {
            Text = "Sair",
            Font = new Font("Segoe UI", 10F),
            ForeColor = corTexto,
            BackColor = corCard,
            FlatStyle = FlatStyle.Flat,
            Height = 40,
            Dock = DockStyle.Bottom,
            Margin = new Padding(0, 5, 0, 0),
            Cursor = Cursors.Hand
        };
        btnSair.FlatAppearance.BorderColor = corBorda;
        btnSair.FlatAppearance.BorderSize = 1;
        btnSair.Click += btnSair_Click;

        panelLateral.Controls.Add(btnSair);
        panelLateral.Controls.Add(btnDominio);
        panelLateral.Controls.Add(btnPreparar);
        panelLateral.Controls.Add(sep1);
        panelLateral.Controls.Add(lblVersao);
        panelLateral.Controls.Add(lblDeploy);
        panelLateral.Controls.Add(lblLogo);

        panelTopo = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = corCard,
            Padding = new Padding(25, 15, 25, 15)
        };

        lblTitulo = new Label
        {
            Text = "Dashboard de Preparacao",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(25, 18)
        };

        panelTopo.Controls.Add(lblTitulo);

        panelStatus = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = corFundo,
            Padding = new Padding(25, 15, 25, 10)
        };

        cardAdmin = CriarCard("Administrador", "admin", out lblCardAdmin);
        cardDominio = CriarCard("Dominio", "dominio", out lblCardDominio);
        cardServidor = CriarCard("Servidor", "servidor", out lblCardServidor);

        var flowStatus = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = corFundo,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        flowStatus.Controls.Add(cardAdmin);
        flowStatus.Controls.Add(cardDominio);
        flowStatus.Controls.Add(cardServidor);

        panelStatus.Controls.Add(flowStatus);

        panelConteudo = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = corFundo,
            Padding = new Padding(25, 10, 25, 10)
        };

        var panelInfo = new Panel
        {
            Width = 380,
            Height = 280,
            BackColor = corCard,
            Location = new Point(0, 0)
        };

        var lblInfoTitulo = new Label
        {
            Text = "Informacoes do Sistema",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 12),
            AutoSize = true
        };

        int y = 45;
        int gap = 24;
        (lblComputador, lblComputadorValor) = CriarInfoLabel("Computador:", ref y, panelInfo);
        y += gap;
        (lblUsuario, lblUsuarioValor) = CriarInfoLabel("Usuario:", ref y, panelInfo);
        y += gap;
        (lblWindows, lblWindowsValor) = CriarInfoLabel("Windows:", ref y, panelInfo);
        y += gap;
        (lblDominio, lblDominioValor) = CriarInfoLabel("Dominio:", ref y, panelInfo);
        y += gap;
        (lblIP, lblIPValor) = CriarInfoLabel("Endereco IP:", ref y, panelInfo);
        y += gap;
        (lblMAC, lblMACValor) = CriarInfoLabel("MAC Address:", ref y, panelInfo);
        y += gap;
        (lblProcessador, lblProcessadorValor) = CriarInfoLabel("Processador:", ref y, panelInfo);
        y += gap;
        (lblRAM, lblRAMValor) = CriarInfoLabel("Memoria RAM:", ref y, panelInfo);
        y += gap;
        (lblDisco, lblDiscoValor) = CriarInfoLabel("Disco (C:):", ref y, panelInfo);
        y += gap;
        (lblSerial, lblSerialValor) = CriarInfoLabel("Serial Number:", ref y, panelInfo);

        panelInfo.Controls.Add(lblInfoTitulo);

        panelSoftwares = new Panel
        {
            Width = 480,
            Height = 280,
            BackColor = corCard,
            Location = new Point(400, 0),
            AutoScroll = true
        };

        var lblSwTitulo = new Label
        {
            Text = "Status dos Softwares",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 12),
            AutoSize = true
        };
        panelSoftwares.Controls.Add(lblSwTitulo);

        int swY = 45;
        int swGap = 36;
        swOffice = CriarSwPanel("Microsoft Office", TipoSoftware.Office, ref swY, panelSoftwares); swY += swGap;
        swUltraVNC = CriarSwPanel("UltraVNC", TipoSoftware.UltraVNC, ref swY, panelSoftwares); swY += swGap;
        swChrome = CriarSwPanel("Google Chrome", TipoSoftware.Chrome, ref swY, panelSoftwares); swY += swGap;
        swFirefox = CriarSwPanel("Mozilla Firefox", TipoSoftware.Firefox, ref swY, panelSoftwares); swY += swGap;
        swFoxit = CriarSwPanel("Foxit PDF Reader", TipoSoftware.Foxit, ref swY, panelSoftwares); swY += swGap;
        swWinRAR = CriarSwPanel("WinRAR", TipoSoftware.WinRAR, ref swY, panelSoftwares); swY += swGap;
        swNinite = CriarSwPanel("Ninite Updater", TipoSoftware.Ninite, ref swY, panelSoftwares);

        var panelProgresso = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            BackColor = corFundo,
            Padding = new Padding(0, 10, 0, 0)
        };

        progressBar = new ProgressBar
        {
            Dock = DockStyle.Fill,
            Height = 24,
            Style = ProgressBarStyle.Continuous,
            BackColor = corCard,
            ForeColor = corDestaque
        };

        lblProgresso = new Label
        {
            Text = "0%",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = corDestaque,
            AutoSize = true,
            Dock = DockStyle.Right,
            TextAlign = ContentAlignment.MiddleRight,
            Padding = new Padding(0, 0, 10, 0)
        };

        panelProgresso.Controls.Add(progressBar);
        panelProgresso.Controls.Add(lblProgresso);

        var panelCentro = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = corFundo
        };
        panelCentro.Controls.Add(panelSoftwares);
        panelCentro.Controls.Add(panelInfo);
        panelCentro.Controls.Add(panelProgresso);

        panelConteudo.Controls.Add(panelCentro);

        panelLog = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 200,
            BackColor = corFundo,
            Padding = new Padding(25, 5, 25, 20)
        };

        var lblLogTitulo = new Label
        {
            Text = "Log de Execucao",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = corTexto,
            Dock = DockStyle.Top,
            Height = 24
        };

        txtLog = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BackColor = corCard,
            ForeColor = corTexto,
            Font = new Font("Consolas", 9.5F),
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        panelLog.Controls.Add(txtLog);
        panelLog.Controls.Add(lblLogTitulo);

        Controls.Add(panelConteudo);
        Controls.Add(panelLog);
        Controls.Add(panelStatus);
        Controls.Add(panelTopo);
        Controls.Add(panelLateral);

        Load += FormMain_Load;

        ResumeLayout(false);
        PerformLayout();
    }

    private Panel CriarCard(string titulo, string chave, out Label lbl)
    {
        Color corCard = Color.FromArgb(22, 27, 34);
        Color corTexto = Color.FromArgb(201, 209, 217);

        var panel = new Panel
        {
            Width = 200,
            Height = 60,
            BackColor = corCard,
            Margin = new Padding(0, 0, 12, 0),
            Padding = new Padding(12, 8, 12, 8)
        };

        lbl = new Label
        {
            Text = "  " + titulo,
            Tag = titulo,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = corTexto,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(lbl);
        _cards[chave] = panel;
        _cardLabels[chave] = lbl;
        return panel;
    }

    private (Label lbl, Label lblVal) CriarInfoLabel(string titulo, ref int y, Panel parent)
    {
        Color corTexto = Color.FromArgb(139, 148, 158);
        Color corValor = Color.FromArgb(201, 209, 217);

        var lbl = new Label
        {
            Text = titulo,
            Font = new Font("Segoe UI", 9F),
            ForeColor = corTexto,
            Location = new Point(15, y),
            AutoSize = true
        };

        var lblVal = new Label
        {
            Text = "—",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = corValor,
            Location = new Point(140, y),
            AutoSize = true,
            MaximumSize = new Size(220, 0)
        };

        parent.Controls.Add(lbl);
        parent.Controls.Add(lblVal);
        return (lbl, lblVal);
    }

    private Panel CriarSwPanel(string nome, TipoSoftware tipo, ref int y, Panel parent)
    {
        Color corCard = Color.FromArgb(48, 54, 61);

        var panel = new Panel
        {
            Width = 440,
            Height = 28,
            BackColor = corCard,
            Location = new Point(15, y),
            Margin = new Padding(0, 0, 0, 8)
        };

        var lbl = new Label
        {
            Text = "  " + nome,
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(lbl);
        parent.Controls.Add(panel);
        _softwarePanels[tipo] = panel;
        return panel;
    }
}