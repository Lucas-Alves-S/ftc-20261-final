namespace Parte1;

// Este arquivo foi projetado para ser legível sem documentação auxiliar.
// A necessidade de comentários linha a linha indicaria uma falha de clareza
// na implementação, e não uma deficiência na documentação.
class TransicaoConfig
{
    public string Origem { get; set; } = "";
    public string Simbolo { get; set; } = "";
    public string Destino { get; set; } = "";
}

class AFDConfig
{
    public List<string> Estados { get; set; } = [];
    public List<string> Alfabeto { get; set; } = [];
    public string EstadoInicial { get; set; } = "";
    public List<string> EstadosAceitacao { get; set; } = [];
    public List<TransicaoConfig> Transicoes { get; set; } = [];
}
