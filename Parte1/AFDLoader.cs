using System.Text.Json;

namespace Parte1;

static class AFDLoader
{
    public static AFDConfig? Carregar(string caminho)
    {
        if (!File.Exists(caminho))
            return null;

        var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<AFDConfig>(File.ReadAllText(caminho), opcoes);
    }
}
