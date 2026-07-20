using ILAPEODeploy.Models;
using ILAPEODeploy.Services;

namespace ILAPEODeploy.UI;

public partial class FormPerfil : Form
{
    private PerfilComputador? _perfilSelecionado;
    private int _numeroSugerido = 1;

    public PerfilComputador? PerfilSelecionado => _perfilSelecionado;
    public string NomeComputadorSugerido => _perfilSelecionado != null
        ? _perfilSelecionado.GerarNome(_numeroSugerido)
        : string.Empty;

    private readonly List<PerfilComputador> _perfis = new()
    {
        new() { Id = "notebook",     Nome = "Notebook",           Prefixo = "ILANTB",   NumeroInicial = 1,   NumeroMaximo = 100, Digitos = 3, Descricao = "Notebooks padrao da ILAPEO", Icone = "💻" },
        new() { Id = "desktop",      Nome = "Desktop",            Prefixo = "ILADSK",   NumeroInicial = 1,   NumeroMaximo = 100, Digitos = 3, Descricao = "Desktops fixos", Icone = "🖥️" },
        new() { Id = "workstation",  Nome = "Workstation",        Prefixo = "ILAWKT",   NumeroInicial = 1,   NumeroMaximo = 50,  Digitos = 3, Descricao = "Estacoes de trabalho alto desempenho", Icone = "⚡" },
        new() { Id = "lab_work",     Nome = "Lab. Workstation",   Prefixo = "ILALABWKT",NumeroInicial = 1,   NumeroMaximo = 20,  Digitos = 2, Descricao = "Workstations do laboratorio", Icone = "🔬" },
        new() { Id = "lab",          Nome = "Laboratorio",        Prefixo = "ILALAB",   NumeroInicial = 1,   NumeroMaximo = 50,  Digitos = 3, Descricao = "Computadores do laboratorio", Icone = "🧪" },
        new() { Id = "clinica_lilas",Nome = "Clinica Lilas",      Prefixo = "ILACLL",   NumeroInicial = 1,   NumeroMaximo = 50,  Digitos = 2, Descricao = "Clinica Lilas", Icone = "🟣" },
        new() { Id = "clinica_verde",Nome = "Clinica Verde",      Prefixo = "ILACLV",   NumeroInicial = 1,   NumeroMaximo = 50,  Digitos = 2, Descricao = "Clinica Verde", Icone = "🟢" },
        new() { Id = "clinica_azul", Nome = "Clinica Azul",       Prefixo = "ILACLA",   NumeroInicial = 1,   NumeroMaximo = 50,  Digitos = 2, Descricao = "Clinica Azul", Icone = "🔵" },
        new() { Id = "biblioteca",   Nome = "Biblioteca",         Prefixo = "ILABLB",   NumeroInicial = 1,   NumeroMaximo = 20,  Digitos = 3, Descricao = "Computadores da biblioteca", Icone = "📚" }
    };

    public FormPerfil()
    {
        InitializeComponent();
    }

    private void FormPerfil_Load(object? sender, EventArgs e)
    {
        lblHostnameAtual.Text = $"Hostname atual: {Environment.MachineName}";
        CarregarPerfis();
    }

    private void CarregarPerfis()
    {
        flowPerfis.Controls.Clear();

        foreach (var perfil in _perfis)
        {
            var card = CriarCardPerfil(perfil);
            flowPerfis.Controls.Add(card);
        }
    }

    private Panel CriarCardPerfil(PerfilComputador perfil)
    {
        Color corCard = Color.FromArgb(22, 27, 34);
        Color corCardHover = Color.FromArgb(33, 38, 45);
        Color corBorda = Color.FromArgb(48, 54, 61);
        Color corDestaque = Color.FromArgb(31, 111, 235);

        var panel = new Panel
        {
            Width = 220,
            Height = 120,
            BackColor = corCard,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(10),
            Cursor = Cursors.Hand,
            Tag = perfil
        };

        var lblIcone = new Label
        {
            Text = perfil.Icone,
            Font = new Font("Segoe UI", 28),
            Location = new Point(15, 10),
            AutoSize = true
        };

        var lblNome = new Label
        {
            Text = perfil.Nome,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 55),
            AutoSize = true
        };

        var lblPrefixo = new Label
        {
            Text = perfil.Prefixo + "xxx",
            Font = new Font("Segoe UI", 9),
            ForeColor = corDestaque,
            Location = new Point(15, 80),
            AutoSize = true
        };

        var lblDesc = new Label
        {
            Text = perfil.Descricao,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(139, 148, 158),
            Location = new Point(15, 98),
            AutoSize = true,
            MaximumSize = new Size(190, 0)
        };

        panel.Controls.Add(lblIcone);
        panel.Controls.Add(lblNome);
        panel.Controls.Add(lblPrefixo);
        panel.Controls.Add(lblDesc);

        // Hover effect
        panel.MouseEnter += (s, e) => panel.BackColor = corCardHover;
        panel.MouseLeave += (s, e) => panel.BackColor = corCard;
        foreach (Control c in panel.Controls)
        {
            c.MouseEnter += (s, e) => panel.BackColor = corCardHover;
            c.MouseLeave += (s, e) => panel.BackColor = corCard;
        }

        panel.Click += (s, e) => SelecionarPerfil(perfil);
        foreach (Control c in panel.Controls)
            c.Click += (s, e) => SelecionarPerfil(perfil);

        return panel;
    }

    private void SelecionarPerfil(PerfilComputador perfil)
    {
        _perfilSelecionado = perfil;

        // Busca proximo numero disponivel na rede
        _numeroSugerido = ComputerService.ObterProximoNumeroDisponivel(perfil);

        string nomeSugerido = perfil.GerarNome(_numeroSugerido);

        var result = MessageBox.Show(
            $"Perfil selecionado: {perfil.Nome}\n\n" +
            $"Nome sugerido: {nomeSugerido}\n\n" +
            $"Deseja usar este nome?\n\n" +
            $"Sim = Aplicar nome e continuar\n" +
            $"Nao = Escolher outro numero",
            "Confirmar Nome do Computador",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            // Aplica o nome e fecha
            DialogResult = DialogResult.OK;
            Close();
        }
        else if (result == DialogResult.No)
        {
            // Pede numero customizado
            using var input = new FormInputNumero(perfil, _numeroSugerido);
            if (input.ShowDialog() == DialogResult.OK)
            {
                _numeroSugerido = input.NumeroEscolhido;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        // Cancel = volta para selecionar outro perfil
    }

    private void btnPular_Click(object? sender, EventArgs e)
    {
        _perfilSelecionado = null;
        DialogResult = DialogResult.Ignore; // Pular sem renomear
        Close();
    }
}
