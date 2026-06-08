namespace Parte3;

// Este arquivo foi projetado para ser legível sem documentação auxiliar.
// A necessidade de comentários linha a linha indicaria uma falha de clareza
// na implementação, e não uma deficiência na documentação.
class TransicaoMTConfig
{
    public string Origem { get; set; } = "";
    public string Simbolo { get; set; } = "";
    public string NovoEstado { get; set; } = "";
    public string NovoSimbolo { get; set; } = "";
    public string Direcao { get; set; } = "";
}

class MTConfig
{
    public List<string> Estados { get; set; } = [];
    public List<string> AlfabetoEntrada { get; set; } = [];
    public List<string> AlfabetoFita { get; set; } = [];
    public string EstadoInicial { get; set; } = "";
    public string EstadoAceitacao { get; set; } = "";
    public string EstadoRejeicao { get; set; } = "";
    public int LimitePassos { get; set; } = 1000;
    public List<TransicaoMTConfig> Transicoes { get; set; } = [];
}
