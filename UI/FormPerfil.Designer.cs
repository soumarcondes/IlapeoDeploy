#nullable disable
using ILAPEODeploy.Core;

namespace ILAPEODeploy.UI;

partial class FormPerfil
{
    private System.ComponentModel.IContainer components = null;

    private Panel panelTopo;
    private Panel panelBotoes;
    private FlowLayoutPanel flowPerfis;
    private Label lblTitulo;
    private Label lblSubtitulo;
    private Label lblHostnameAtual;
    private Button btnPular;

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

        SuspendLayout();
        BackColor = corFundo;
        ForeColor = corTexto;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ClientSize = new Size(900, 600);
        MinimumSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;
        Text = $"{VersionInfo.Nome} — Selecionar Perfil";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // Topo
        panelTopo = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = corCard,
            Padding = new Padding(30, 20, 30, 20)
        };

        lblTitulo = new Label
        {
            Text = "Selecione o Perfil do Computador",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 15)
        };

        lblSubtitulo = new Label
        {
            Text = "Escolha o setor para definir o padrao de nomenclatura",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(139, 148, 158),
            AutoSize = true,
            Location = new Point(30, 50)
        };

        lblHostnameAtual = new Label
        {
            Text = "Hostname atual: —",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(31, 111, 235),
            AutoSize = true,
            Location = new Point(30, 75)
        };

        panelTopo.Controls.Add(lblTitulo);
        panelTopo.Controls.Add(lblSubtitulo);
        panelTopo.Controls.Add(lblHostnameAtual);

        // Flow de perfis
        flowPerfis = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = corFundo,
            Padding = new Padding(20),
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        // Painel de botoes
        panelBotoes = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = corCard,
            Padding = new Padding(20, 10, 20, 10)
        };

        btnPular = new Button
        {
            Text = "Pular (manter nome atual)",
            Font = new Font("Segoe UI", 10F),
            ForeColor = corTexto,
            BackColor = corCard,
            FlatStyle = FlatStyle.Flat,
            Width = 220,
            Height = 38,
            Dock = DockStyle.Right,
            Cursor = Cursors.Hand
        };
        btnPular.FlatAppearance.BorderColor = corBorda;
        btnPular.FlatAppearance.BorderSize = 1;
        btnPular.Click += btnPular_Click;

        panelBotoes.Controls.Add(btnPular);

        Controls.Add(flowPerfis);
        Controls.Add(panelBotoes);
        Controls.Add(panelTopo);

        Load += FormPerfil_Load;

        ResumeLayout(false);
        PerformLayout();
    }
}
