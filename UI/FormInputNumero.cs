using ILAPEODeploy.Models;

namespace ILAPEODeploy.UI;

public partial class FormInputNumero : Form
{
    public int NumeroEscolhido { get; private set; }
    private readonly PerfilComputador _perfil;
    private readonly int _sugerido;

    public FormInputNumero(PerfilComputador perfil, int sugerido)
    {
        _perfil = perfil;
        _sugerido = sugerido;
        InitializeComponent();
    }

    private void FormInputNumero_Load(object? sender, EventArgs e)
    {
        lblPerfil.Text = $"Perfil: {_perfil.Nome}";
        lblPrefixo.Text = $"Prefixo: {_perfil.Prefixo}";
        lblRange.Text = $"Range: {_perfil.NumeroInicial} a {_perfil.NumeroMaximo}";
        numInput.Value = _sugerido;
        numInput.Minimum = _perfil.NumeroInicial;
        numInput.Maximum = _perfil.NumeroMaximo;
        AtualizarPreview();
    }

    private void numInput_ValueChanged(object? sender, EventArgs e)
    {
        AtualizarPreview();
    }

    private void AtualizarPreview()
    {
        int num = (int)numInput.Value;
        lblPreview.Text = $"Preview: {_perfil.GerarNome(num)}";
    }

    private void btnConfirmar_Click(object? sender, EventArgs e)
    {
        NumeroEscolhido = (int)numInput.Value;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
