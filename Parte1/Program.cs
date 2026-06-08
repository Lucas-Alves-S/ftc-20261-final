using Parte1;

// Este arquivo foi projetado para ser legível sem documentação auxiliar.
// A necessidade de comentários linha a linha indicaria uma falha de clareza
// na implementação, e não uma deficiência na documentação.
const string arquivoAfd = "afd.json";
const string arquivoEntradas = "entradas.txt";

AFDConfig? config = AFDLoader.Carregar(arquivoAfd);

AFD afd;
if (config is not null)
{
    Console.WriteLine($"AFD carregado de '{arquivoAfd}'.");
    afd = new AFD(config);
}
else
{
    Console.WriteLine($"'{arquivoAfd}' não encontrado — usando AFD padrão (L1).");
    afd = new AFD();
}

afd.ExibirDiagrama();

if (!File.Exists(arquivoEntradas))
{
    Console.WriteLine($"\nArquivo '{arquivoEntradas}' não encontrado.");
    return;
}

string[] linhas = File.ReadAllLines(arquivoEntradas);
Console.WriteLine($"\n=== Processando '{arquivoEntradas}' ({linhas.Length} linha(s)) ===\n");

for (int i = 0; i < linhas.Length; i++)
{
    string cadeia = linhas[i];
    string exibida = cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"";
    Console.WriteLine($"[{i + 1}] Cadeia: {exibida}");
    afd.Aceitar(cadeia);
    Console.WriteLine();
}
