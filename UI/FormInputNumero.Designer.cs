#nullable disable

namespace ILAPEODeploy.UI;

partial class FormInputNumero
{
    private System.ComponentModel.IContainer components = null;

    private Label lblTitulo;
    private Label lblPerfil;
    private Label lblPrefixo;
    private Label lblRange;
    private Label lblPreview;
    private NumericUpDown numInput;
    private Button btnConfirmar;
    private Button btnCancelar;

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
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(400, 280);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Definir Numero";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        lblTitulo = new Label
        {
            Text = "Definir Numero do Computador",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 15),
            AutoSize = true
        };

        lblPerfil = new Label
        {
            Text = "Perfil: —",
            Location = new Point(20, 50),
            AutoSize = true
        };

        lblPrefixo = new Label
        {
            Text = "Prefixo: —",
            Location = new Point(20, 75),
            AutoSize = true
        };

        lblRange = new Label
        {
            Text = "Range: —",
            Location = new Point(20, 100),
            AutoSize = true
        };

        numInput = new NumericUpDown
        {
            Location = new Point(20, 135),
            Width = 120,
            Height = 28,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = corDestaque,
            BackColor = corCard,
            BorderStyle = BorderStyle.FixedSingle
        };
        numInput.ValueChanged += numInput_ValueChanged;

        lblPreview = new Label
        {
            Text = "Preview: —",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = corDestaque,
            Location = new Point(20, 175),
            AutoSize = true
        };

        btnConfirmar = new Button
        {
            Text = "Confirmar",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = corBotao,
            FlatStyle = FlatStyle.Flat,
            Width = 120,
            Height = 36,
            Location = new Point(20, 220),
            Cursor = Cursors.Hand
        };
        btnConfirmar.FlatAppearance.BorderSize = 0;
        btnConfirmar.Click += btnConfirmar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Font = new Font("Segoe UI", 10F),
            ForeColor = corTexto,
            BackColor = corCard,
            FlatStyle = FlatStyle.Flat,
            Width = 120,
            Height = 36,
            Location = new Point(150, 220),
            Cursor = Cursors.Hand
        };
        btnCancelar.FlatAppearance.BorderColor = corBorda;
        btnCancelar.FlatAppearance.BorderSize = 1;
        btnCancelar.Click += btnCancelar_Click;

        Controls.Add(lblTitulo);
        Controls.Add(lblPerfil);
        Controls.Add(lblPrefixo);
        Controls.Add(lblRange);
        Controls.Add(numInput);
        Controls.Add(lblPreview);
        Controls.Add(btnConfirmar);
        Controls.Add(btnCancelar);

        Load += FormInputNumero_Load;

        ResumeLayout(false);
        PerformLayout();
    }
}
