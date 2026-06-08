using System.Text.Json;

namespace Parte3;

// Este arquivo foi projetado para ser legível sem documentação auxiliar.
// A necessidade de comentários linha a linha indicaria uma falha de clareza
// na implementação, e não uma deficiência na documentação.
static class MTLoader
{
    public static MTConfig? Carregar(string caminho)
    {
        if (!File.Exists(caminho))
            return null;

        var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<MTConfig>(File.ReadAllText(caminho), opcoes);
    }
}
