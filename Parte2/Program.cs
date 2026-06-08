using Parte2;

Console.WriteLine("=== Autômatos de Pilha ===");

// --------------------------
// L2 = { a^n b^n | n >= 1 }
// --------------------------
var apL2 = new AP(
    q: new[] { "q0", "q1", "q2" },
    sigma: new[] { 'a', 'b' },
    gamma: new[] { 'A', 'Z' },
    q0: "q0",
    z0: 'Z',
    f: Array.Empty<string>() // Aceitação por pilha vazia
);

// Transições L2
apL2.AdicionarTransicao("q0", 'a', 'Z', "q0", "AZ");

apL2.AdicionarTransicao("q0", 'a', 'A', "q0", "AA");

apL2.AdicionarTransicao("q0", 'b', 'A', "q1", "");

apL2.AdicionarTransicao("q1", 'b', 'A', "q1", "");

// λ-movimento para esvaziar a pilha ('Z' -> ε)
apL2.AdicionarTransicao("q1", '\0', 'Z', "q2", "");

// -----------------------------------------------------
// L3 = { w ∈ {a, b}* | w = w^R, |w| >= 1 } (Palíndromos)
// -----------------------------------------------------
var apL3 = new AP(
    q: new[] { "q0", "q1", "q2" },
    sigma: new[] { 'a', 'b' },
    gamma: new[] { 'A', 'B', 'Z' },
    q0: "q0",
    z0: 'Z',
    f: Array.Empty<string>()
);

// Transições L3 (Não-determinístico)
// Fase de empilhamento (q0)
apL3.AdicionarTransicao("q0", 'a', 'Z', "q0", "AZ");
apL3.AdicionarTransicao("q0", 'b', 'Z', "q0", "BZ");
apL3.AdicionarTransicao("q0", 'a', 'A', "q0", "AA");
apL3.AdicionarTransicao("q0", 'b', 'A', "q0", "BA");
apL3.AdicionarTransicao("q0", 'a', 'B', "q0", "AB");
apL3.AdicionarTransicao("q0", 'b', 'B', "q0", "BB");

apL3.AdicionarTransicao("q0", 'a', 'Z', "q1", "Z");
apL3.AdicionarTransicao("q0", 'b', 'Z', "q1", "Z");
apL3.AdicionarTransicao("q0", 'a', 'A', "q1", "A");
apL3.AdicionarTransicao("q0", 'b', 'A', "q1", "A");
apL3.AdicionarTransicao("q0", 'a', 'B', "q1", "B");
apL3.AdicionarTransicao("q0", 'b', 'B', "q1", "B");

apL3.AdicionarTransicao("q0", '\0', 'A', "q1", "A");
apL3.AdicionarTransicao("q0", '\0', 'B', "q1", "B");

apL3.AdicionarTransicao("q1", 'a', 'A', "q1", "");
apL3.AdicionarTransicao("q1", 'b', 'B', "q1", "");

// Esvaziar a pilha para aceitar (q1 -> q2)
apL3.AdicionarTransicao("q1", '\0', 'Z', "q2", "");

// --------------------------
// Simulação Entradas L2 e L3
// --------------------------
string arquivoEntradas = "entradas_ap.txt";
string arquivoEntradasL3 = "entradas_ap_l3.txt";

Console.WriteLine("\n\n=== Testando L2 (a^n b^n | n >= 1) ===");
foreach (var linha in File.ReadAllLines(arquivoEntradas))
{
    string exibida = linha.Length == 0 ? "ε (vazia)" : $"\"{linha}\"";
    Console.WriteLine($"\n[L2] Cadeia: {exibida}");
    apL2.Aceitar(linha);
}

Console.WriteLine("\n\n=== Testando L3 (Palíndromos |w| >= 1) ===");
foreach (var linha in File.ReadAllLines(arquivoEntradasL3))
{
    string exibida = linha.Length == 0 ? "ε (vazia)" : $"\"{linha}\"";
    Console.WriteLine($"\n[L3] Cadeia: {exibida}");
    apL3.Aceitar(linha);
}
