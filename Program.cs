using System;
using System.Windows.Forms;
using ILAPEODeploy.Services;

namespace ILAPEODeploy.UI;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Etapa 1: Selecionar perfil e nome do computador
        using (var formPerfil = new FormPerfil())
        {
            var resultado = formPerfil.ShowDialog();

            if (resultado == DialogResult.OK && formPerfil.PerfilSelecionado != null)
            {
                string novoNome = formPerfil.NomeComputadorSugerido;
                string nomeAtual = Environment.MachineName;

                if (!nomeAtual.Equals(novoNome, StringComparison.OrdinalIgnoreCase))
                {
                    var confirm = MessageBox.Show(
                        $"O computador sera renomeado de:\n" +
                        $"  {nomeAtual}\n" +
                        $"para:\n" +
                        $"  {novoNome}\n\n" +
                        $"E necessario reiniciar apos a renomeacao.\n" +
                        $"Deseja continuar?",
                        "Confirmar Renomeacao",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        bool ok = ComputerService.RenomearComputador(novoNome, out string msg);

                        MessageBox.Show(msg, "Renomeacao",
                            ok ? MessageBoxButtons.OK : MessageBoxButtons.OK,
                            ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                        if (ok)
                        {
                            // Pergunta se quer reiniciar agora
                            var reboot = MessageBox.Show(
                                "Reiniciar agora para aplicar o novo nome?\n\n" +
                                "Sim = Reinicia agora\n" +
                                "Nao = Continua (reinicie manualmente depois)",
                                "Reiniciar",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (reboot == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("shutdown", "/r /t 5 /c \"Reiniciando apos renomeacao ILAPEO Deploy\"");
                                return;
                            }
                        }
                    }
                }
            }
            // Se Ignore (Pular) ou Cancel, continua sem renomear
        }

        // Etapa 2: Dashboard principal
        Application.Run(new FormMain());
    }
}