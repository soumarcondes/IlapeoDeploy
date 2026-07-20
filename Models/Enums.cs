namespace ILAPEODeploy.Models;

public enum TipoSoftware
{
    Office,
    UltraVNC,
    Chrome,
    Firefox,
    Foxit,
    WinRAR,
    Ninite
}

public enum StatusInstalacao
{
    Pendente,
    Verificando,
    Instalando,
    Concluido,
    Erro,
    Ignorado
}
