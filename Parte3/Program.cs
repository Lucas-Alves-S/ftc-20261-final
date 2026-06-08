using Parte3;

const string arquivoMtL4 = "mt_l4.json";
const string arquivoMtUnario = "mt_unario.json";
const string arquivoEntradasL4 = "entradas_l4.txt";
const string arquivoEntradasUnario = "entradas_unario.txt";

MTConfig? configL4 = MTLoader.Carregar(arquivoMtL4);

MT mtL4;
if (configL4 is not null)
{
    Console.WriteLine($"MT carregada de '{arquivoMtL4}'.");
    mtL4 = new MT(configL4);
}
else
{
    Console.WriteLine($"'{arquivoMtL4}' não encontrado — usando MT padrão (L4).");
    mtL4 = new MT();
}

mtL4.ExibirDiagrama();

if (!File.Exists(arquivoEntradasL4))
{
    Console.WriteLine($"\nArquivo '{arquivoEntradasL4}' não encontrado.");
}
else
{
    string[] linhasL4 = File.ReadAllLines(arquivoEntradasL4);
    Console.WriteLine(
        $"\n=== Processando '{arquivoEntradasL4}' ({linhasL4.Length} linha(s)) ===\n"
    );

    for (int i = 0; i < linhasL4.Length; i++)
    {
        string cadeia = linhasL4[i];
        string exibida = cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"";
        Console.WriteLine($"[{i + 1}] Cadeia: {exibida}");
        mtL4.Aceitar(cadeia);
        Console.WriteLine();
    }
}

Console.WriteLine(new string('═', 60));

MTConfig? configUnario = MTLoader.Carregar(arquivoMtUnario);

MT mtUnario;
if (configUnario is not null)
{
    Console.WriteLine($"MT carregada de '{arquivoMtUnario}'.");
    mtUnario = new MT(configUnario);
}
else
{
    Console.WriteLine($"'{arquivoMtUnario}' não encontrado — usando MT padrão (f(n) = n+1).");
    mtUnario = new MT(ConfiguracaoUnarioPadrao());
}

mtUnario.ExibirDiagrama();

if (!File.Exists(arquivoEntradasUnario))
{
    Console.WriteLine($"\nArquivo '{arquivoEntradasUnario}' não encontrado.");
}
else
{
    string[] linhasUnario = File.ReadAllLines(arquivoEntradasUnario);
    Console.WriteLine(
        $"\n=== Processando '{arquivoEntradasUnario}' ({linhasUnario.Length} linha(s)) ===\n"
    );

    for (int i = 0; i < linhasUnario.Length; i++)
    {
        string cadeia = linhasUnario[i];
        string exibida = cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"";
        Console.WriteLine($"[{i + 1}] Fita de entrada: {exibida}");
        mtUnario.Aceitar(cadeia);
        Console.WriteLine();
    }
}

static MTConfig ConfiguracaoUnarioPadrao()
{
    // TODO: Retornar um MTConfig para a MT que calcula f(n) = n+1 em notação unária.
    //
    // Especificação:
    // - Estados:          ["q0", "qaccept", "qreject"]
    // - AlfabetoEntrada:  ["1"]
    // - AlfabetoFita:     ["1", "_"]
    // - EstadoInicial:    "q0"
    // - EstadoAceitacao:  "qaccept"
    // - EstadoRejeicao:   "qreject"
    // - LimitePassos:     10000
    //
    // Transições (2 regras):
    //   (q0, '1') → (q0,  '1', R)  — percorre os 1s existentes sem alterar
    //   (q0, '_') → (qaccept, '1', R)  — ao encontrar o branco, escreve '1' e aceita
    //
    // Efeito: uma fita com n '1's termina com n+1 '1's.
    throw new NotImplementedException("TODO: ConfiguracaoUnarioPadrao");
}
