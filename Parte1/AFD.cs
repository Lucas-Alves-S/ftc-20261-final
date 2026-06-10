namespace Parte1;

// Modela a 5-tupla M = (Q, Σ, δ, q0, F)
class AFD
{
    private readonly HashSet<string> _q;
    private readonly HashSet<char> _sigma;
    private readonly Dictionary<(string estado, char simbolo), string> _delta;
    private readonly string _q0;
    private readonly HashSet<string> _f;

    public IReadOnlySet<string> Q => _q;
    public IReadOnlySet<char> Sigma => _sigma;
    public IReadOnlyDictionary<(string estado, char simbolo), string> Delta => _delta;
    public string Q0 => _q0;
    public IReadOnlySet<string> F => _f;

    // Sem configuração → fallback L1 = { w ∈ {a,b}^* | w termina com 'ab' }
    public AFD()
        : this(ConfiguracaoL1Padrao()) { }

    public AFD(AFDConfig config)
    {
        _q = new HashSet<string>(config.Estados);
        _sigma = new HashSet<char>(config.Alfabeto.Where(s => s.Length == 1).Select(s => s[0]));
        _delta = [];
        _q0 = config.EstadoInicial;
        _f = new HashSet<string>(config.EstadosAceitacao);

        foreach (var t in config.Transicoes)
            if (t.Simbolo.Length == 1)
                _delta[(t.Origem, t.Simbolo[0])] = t.Destino;
    }

    private static AFDConfig ConfiguracaoL1Padrao() =>
        new()
        {
            Estados = ["q0", "q1", "q2"],
            Alfabeto = ["a", "b"],
            EstadoInicial = "q0",
            EstadosAceitacao = ["q2"],
            Transicoes =
            [
                // δ(q0,'a')=q1 — leu 'a'; pode ser início de 'ab'
                new()
                {
                    Origem = "q0",
                    Simbolo = "a",
                    Destino = "q1",
                },
                // δ(q0,'b')=q0 — leu 'b' sem 'a' anterior; nenhum progresso
                new()
                {
                    Origem = "q0",
                    Simbolo = "b",
                    Destino = "q0",
                },
                // δ(q1,'a')=q1 — novo 'a'; mantém a esperança de completar 'ab'
                new()
                {
                    Origem = "q1",
                    Simbolo = "a",
                    Destino = "q1",
                },
                // δ(q1,'b')=q2 — leu 'b' após 'a'; completou sufixo 'ab'
                new()
                {
                    Origem = "q1",
                    Simbolo = "b",
                    Destino = "q2",
                },
                // δ(q2,'a')=q1 — leu 'a' após 'ab'; possível novo sufixo
                new()
                {
                    Origem = "q2",
                    Simbolo = "a",
                    Destino = "q1",
                },
                // δ(q2,'b')=q0 — leu 'b' após 'ab'; sufixo quebrado
                new()
                {
                    Origem = "q2",
                    Simbolo = "b",
                    Destino = "q0",
                },
            ],
        };

    public bool Aceitar(string cadeia)
    {
        string estadoAtual = _q0;
        var rastro = new List<string> { estadoAtual };

        foreach (char c in cadeia)
        {
            // aplica δ(estadoAtual, c); ausência na tabela indica c ∉ Σ
            if (!_delta.TryGetValue((estadoAtual, c), out string? proximo))
            {
                Console.WriteLine($"  Rastro    : {string.Join(" -> ", rastro)} -> ERRO");
                Console.WriteLine($"  Motivo    : símbolo '{c}' não pertence ao alfabeto");
                Console.WriteLine($"  Resultado : REJEITA");
                return false;
            }
            estadoAtual = proximo; // estadoAtual = δ(estadoAtual, c) — passo de transição
            rastro.Add(estadoAtual);
        }

        bool aceita = _f.Contains(estadoAtual);
        Console.WriteLine($"  Rastro    : {string.Join(" -> ", rastro)}");
        Console.WriteLine($"  Resultado : {(aceita ? "ACEITA" : "REJEITA")}");
        return aceita;
    }

    public void ExibirDiagrama()
    {
        var estados = _q.OrderBy(e => e).ToList();
        var simbolos = _sigma.OrderBy(s => s).ToList();

        int larguraEstado = estados.Max(e => e.Length) + 5; // +5 reserva espaço para o prefixo "→ * "
        int larguraSimbolo = Math.Max(simbolos.Max(_ => 1), 5);

        Console.WriteLine();
        Console.WriteLine("=== Tabela de Transições ===");

        string cabecalho =
            "Estado".PadRight(larguraEstado)
            + "| "
            + string.Join(" | ", simbolos.Select(s => s.ToString().PadRight(larguraSimbolo)));
        Console.WriteLine(cabecalho);
        Console.WriteLine(
            new string('-', larguraEstado)
                + "+"
                + string.Join("-+-", simbolos.Select(_ => new string('-', larguraSimbolo + 1)))
        );

        foreach (string estado in estados)
        {
            // prefixo identifica estado inicial (→) e estados de aceitação (*)
            string marcador = (estado == _q0 ? "→ " : "  ") + (_f.Contains(estado) ? "* " : "  ");
            string linha =
                (marcador + estado).PadRight(larguraEstado)
                + "| "
                + string.Join(
                    " | ",
                    simbolos.Select(s =>
                        // consulta δ(estado, símbolo); "-" se a transição não estiver definida
                        _delta.TryGetValue((estado, s), out string? dest)
                            ? dest.PadRight(larguraSimbolo)
                            : "-".PadRight(larguraSimbolo)
                    )
                );
            Console.WriteLine(linha);
        }

        Console.WriteLine();
        Console.WriteLine("  →  = estado inicial     *  = estado de aceitação");
    }
}
