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
// δ(q0,a,Z)=(q0,AZ) — primeiro 'a'; empilha A mantendo base Z
apL2.AdicionarTransicao("q0", 'a', 'Z', "q0", "AZ");

// δ(q0,a,A)=(q0,AA) — mais um 'a'; empilha A
apL2.AdicionarTransicao("q0", 'a', 'A', "q0", "AA");

// δ(q0,b,A)=(q1,ε) — primeiro 'b'; desempilha A e entra na fase de casamento
apL2.AdicionarTransicao("q0", 'b', 'A', "q1", "");

// δ(q1,b,A)=(q1,ε) — outro 'b'; desempilha A (cada 'b' cancela um 'a')
apL2.AdicionarTransicao("q1", 'b', 'A', "q1", "");

// δ(q1,λ,Z)=(q2,ε) — λ: base Z encontrada; desempilha Z e aceita por pilha vazia
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
// δ(q0,a,Z)=(q0,AZ) — lê 'a' sobre base Z; empilha A (ainda não decidiu o meio)
apL3.AdicionarTransicao("q0", 'a', 'Z', "q0", "AZ");

// δ(q0,b,Z)=(q0,BZ) — lê 'b' sobre base Z; empilha B
apL3.AdicionarTransicao("q0", 'b', 'Z', "q0", "BZ");

// δ(q0,a,A)=(q0,AA) — lê 'a' com A no topo; empilha A
apL3.AdicionarTransicao("q0", 'a', 'A', "q0", "AA");

// δ(q0,b,A)=(q0,BA) — lê 'b' com A no topo; empilha B
apL3.AdicionarTransicao("q0", 'b', 'A', "q0", "BA");

// δ(q0,a,B)=(q0,AB) — lê 'a' com B no topo; empilha A
apL3.AdicionarTransicao("q0", 'a', 'B', "q0", "AB");

// δ(q0,b,B)=(q0,BB) — lê 'b' com B no topo; empilha B
apL3.AdicionarTransicao("q0", 'b', 'B', "q0", "BB");

// δ(q0,a,Z)=(q1,Z) — 'a' é o símbolo central (comprimento ímpar); avança para desempilhamento
apL3.AdicionarTransicao("q0", 'a', 'Z', "q1", "Z");

// δ(q0,b,Z)=(q1,Z) — 'b' é o símbolo central (comprimento ímpar); avança para desempilhamento
apL3.AdicionarTransicao("q0", 'b', 'Z', "q1", "Z");

// δ(q0,a,A)=(q1,A) — 'a' central com A no topo; avança para desempilhamento
apL3.AdicionarTransicao("q0", 'a', 'A', "q1", "A");

// δ(q0,b,A)=(q1,A) — 'b' central com A no topo; avança para desempilhamento
apL3.AdicionarTransicao("q0", 'b', 'A', "q1", "A");

// δ(q0,a,B)=(q1,B) — 'a' central com B no topo; avança para desempilhamento
apL3.AdicionarTransicao("q0", 'a', 'B', "q1", "B");

// δ(q0,b,B)=(q1,B) — 'b' central com B no topo; avança para desempilhamento
apL3.AdicionarTransicao("q0", 'b', 'B', "q1", "B");

// δ(q0,λ,A)=(q1,A) — λ: comprimento par; meio alcançado com A no topo, inicia desempilhamento
apL3.AdicionarTransicao("q0", '\0', 'A', "q1", "A");

// δ(q0,λ,B)=(q1,B) — λ: comprimento par; meio alcançado com B no topo, inicia desempilhamento
apL3.AdicionarTransicao("q0", '\0', 'B', "q1", "B");

// δ(q1,a,A)=(q1,ε) — 'a' casa com A; desempilha (simetria confirmada)
apL3.AdicionarTransicao("q1", 'a', 'A', "q1", "");

// δ(q1,b,B)=(q1,ε) — 'b' casa com B; desempilha (simetria confirmada)
apL3.AdicionarTransicao("q1", 'b', 'B', "q1", "");

// δ(q1,λ,Z)=(q2,ε) — λ: base Z atingida; desempilha Z e aceita por pilha vazia
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
