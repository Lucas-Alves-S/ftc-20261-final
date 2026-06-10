namespace Parte3;

// Modela a 7-tupla M = (Q, Σ, Γ, δ, q0, qaccept, qreject)
class MT
{
    private readonly HashSet<string> _q;
    private readonly HashSet<char> _gamma;
    private readonly Dictionary<
        (string estado, char simbolo),
        (string novoEstado, char novoSimbolo, char direcao)
    > _delta;
    private readonly string _q0;
    private readonly string _qaccept;
    private readonly string _qreject;
    private readonly int _limitePassos;

    public const char Branco = '_';

    // Sem configuração → fallback L4 = { aⁿbⁿcⁿ | n ≥ 1 }
    public MT()
        : this(ConfiguracaoL4Padrao()) { }

    public MT(MTConfig config)
    {
        _q = new HashSet<string>(config.Estados);
        _gamma = new HashSet<char>(config.AlfabetoFita.Where(s => s.Length == 1).Select(s => s[0]));
        _delta = [];
        _q0 = config.EstadoInicial;
        _qaccept = config.EstadoAceitacao;
        _qreject = config.EstadoRejeicao;
        _limitePassos = config.LimitePassos > 0 ? config.LimitePassos : 1000;

        foreach (var t in config.Transicoes)
            if (t.Simbolo.Length == 1 && t.NovoSimbolo.Length == 1 && t.Direcao.Length == 1)
                _delta[(t.Origem, t.Simbolo[0])] = (t.NovoEstado, t.NovoSimbolo[0], t.Direcao[0]);
    }

    private static MTConfig ConfiguracaoL4Padrao() =>
        new()
        {
            Estados = ["q0", "q1", "q2", "q3", "q4", "qaccept", "qreject"],
            AlfabetoEntrada = ["a", "b", "c"],
            AlfabetoFita = ["a", "b", "c", "A", "B", "C", "_"],
            EstadoInicial = "q0",
            EstadoAceitacao = "qaccept",
            EstadoRejeicao = "qreject",
            LimitePassos = 10000,
            Transicoes =
            [
                // q0: varre à direita procurando 'a' não marcado
                // δ(q0,A)=(q0,A,R) — pula 'A' já marcado; continua buscando 'a'
                new()
                {
                    Origem = "q0",
                    Simbolo = "A",
                    NovoEstado = "q0",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                // δ(q0,a)=(q1,A,R) — marca 'a'→'A'; inicia rodada de marcação
                new()
                {
                    Origem = "q0",
                    Simbolo = "a",
                    NovoEstado = "q1",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                // δ(q0,B)=(q4,B,R) — sem mais 'a' para marcar; passa para verificação final
                new()
                {
                    Origem = "q0",
                    Simbolo = "B",
                    NovoEstado = "q4",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                // δ(q0,_)=(qreject,_,R) — fita vazia; n=0 não satisfaz n≥1
                new()
                {
                    Origem = "q0",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // δ(q0,b)=(qreject,b,R) — 'b' antes de 'a'; ordem inválida
                new()
                {
                    Origem = "q0",
                    Simbolo = "b",
                    NovoEstado = "qreject",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                // δ(q0,c)=(qreject,c,R) — 'c' antes de 'a'; ordem inválida
                new()
                {
                    Origem = "q0",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                // δ(q0,C)=(qreject,C,R) — 'C' marcado sem 'A' correspondente; inválido
                new()
                {
                    Origem = "q0",
                    Simbolo = "C",
                    NovoEstado = "qreject",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                // q1: marcou a→A; varre à direita procurando 'b' não marcado
                // δ(q1,a)=(q1,a,R) — pula 'a' não marcado; ainda na zona dos a's
                new()
                {
                    Origem = "q1",
                    Simbolo = "a",
                    NovoEstado = "q1",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
                // δ(q1,B)=(q1,B,R) — pula 'B' já marcado; busca próximo 'b'
                new()
                {
                    Origem = "q1",
                    Simbolo = "B",
                    NovoEstado = "q1",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                // δ(q1,b)=(q2,B,R) — marca 'b'→'B'; par a-b encontrado
                new()
                {
                    Origem = "q1",
                    Simbolo = "b",
                    NovoEstado = "q2",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                // δ(q1,c)=(qreject,c,R) — 'c' antes de 'b'; ordem inválida
                new()
                {
                    Origem = "q1",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                // δ(q1,C)=(qreject,C,R) — 'C' marcado sem 'b' correspondente; inválido
                new()
                {
                    Origem = "q1",
                    Simbolo = "C",
                    NovoEstado = "qreject",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                // δ(q1,_)=(qreject,_,R) — fita acabou sem 'b'; desequilíbrio a>b
                new()
                {
                    Origem = "q1",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q2: marcou b→B; varre à direita procurando 'c' não marcado
                // δ(q2,b)=(q2,b,R) — pula 'b' não marcado; ainda na zona dos b's
                new()
                {
                    Origem = "q2",
                    Simbolo = "b",
                    NovoEstado = "q2",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                // δ(q2,C)=(q2,C,R) — pula 'C' já marcado; busca próximo 'c'
                new()
                {
                    Origem = "q2",
                    Simbolo = "C",
                    NovoEstado = "q2",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                // δ(q2,c)=(q3,C,L) — marca 'c'→'C'; trípla a-b-c marcada; retorna ao início
                new()
                {
                    Origem = "q2",
                    Simbolo = "c",
                    NovoEstado = "q3",
                    NovoSimbolo = "C",
                    Direcao = "L",
                },
                // δ(q2,a)=(qreject,a,R) — 'a' intercalado na zona dos b's; inválido
                new()
                {
                    Origem = "q2",
                    Simbolo = "a",
                    NovoEstado = "qreject",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
                // δ(q2,A)=(qreject,A,R) — 'A' marcado na zona de b's; estrutura inválida
                new()
                {
                    Origem = "q2",
                    Simbolo = "A",
                    NovoEstado = "qreject",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                // δ(q2,_)=(qreject,_,R) — fita acabou sem 'c'; desequilíbrio b>c
                new()
                {
                    Origem = "q2",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q3: marcou c→C; varre à esquerda de volta ao branco inicial
                // δ(q3,C)=(q3,C,L) — pula 'C' marcado; segue voltando à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "C",
                    NovoEstado = "q3",
                    NovoSimbolo = "C",
                    Direcao = "L",
                },
                // δ(q3,c)=(q3,c,L) — pula 'c' não marcado; segue à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "c",
                    NovoEstado = "q3",
                    NovoSimbolo = "c",
                    Direcao = "L",
                },
                // δ(q3,B)=(q3,B,L) — pula 'B' marcado; segue voltando à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "B",
                    NovoEstado = "q3",
                    NovoSimbolo = "B",
                    Direcao = "L",
                },
                // δ(q3,b)=(q3,b,L) — pula 'b' não marcado; segue à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "b",
                    NovoEstado = "q3",
                    NovoSimbolo = "b",
                    Direcao = "L",
                },
                // δ(q3,A)=(q3,A,L) — pula 'A' marcado; segue voltando à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "A",
                    NovoEstado = "q3",
                    NovoSimbolo = "A",
                    Direcao = "L",
                },
                // δ(q3,a)=(q3,a,L) — pula 'a' não marcado; segue à esquerda
                new()
                {
                    Origem = "q3",
                    Simbolo = "a",
                    NovoEstado = "q3",
                    NovoSimbolo = "a",
                    Direcao = "L",
                },
                // δ(q3,_)=(q0,_,R) — chegou ao branco inicial; reinicia iteração
                new()
                {
                    Origem = "q3",
                    Simbolo = "_",
                    NovoEstado = "q0",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q4: sem mais a's; verifica que só restam B's, C's e branco
                // δ(q4,B)=(q4,B,R) — pula 'B' marcado; verifica se resta algo desmarcado
                new()
                {
                    Origem = "q4",
                    Simbolo = "B",
                    NovoEstado = "q4",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                // δ(q4,C)=(q4,C,R) — pula 'C' marcado; verifica se resta algo desmarcado
                new()
                {
                    Origem = "q4",
                    Simbolo = "C",
                    NovoEstado = "q4",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                // δ(q4,_)=(qaccept,_,R) — apenas marcados restaram; aceita
                new()
                {
                    Origem = "q4",
                    Simbolo = "_",
                    NovoEstado = "qaccept",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // δ(q4,b)=(qreject,b,R) — 'b' desmarcado sobrou; desequilíbrio
                new()
                {
                    Origem = "q4",
                    Simbolo = "b",
                    NovoEstado = "qreject",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                // δ(q4,c)=(qreject,c,R) — 'c' desmarcado sobrou; desequilíbrio
                new()
                {
                    Origem = "q4",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                // δ(q4,a)=(qreject,a,R) — 'a' desmarcado na fase final; inválido
                new()
                {
                    Origem = "q4",
                    Simbolo = "a",
                    NovoEstado = "qreject",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
                // δ(q4,A)=(qreject,A,R) — 'A' marcado sem par B/C; inválido
                new()
                {
                    Origem = "q4",
                    Simbolo = "A",
                    NovoEstado = "qreject",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
            ],
        };

    public bool Aceitar(string cadeia)
    {
        // fita inicializada com a cadeia de entrada; posições ausentes lidas como ⊔
        var fita = new Dictionary<int, char>();
        for (int i = 0; i < cadeia.Length; i++)
            fita[i] = cadeia[i];

        string estadoAtual = _q0;
        int cabecote = 0;
        int passos = 0;

        while (true)
        {
            ExibirPasso(passos, estadoAtual, fita, cabecote);

            if (estadoAtual == _qaccept || estadoAtual == _qreject)
                break;

            if (passos >= _limitePassos)
            {
                Console.WriteLine($"  Resultado : LIMITE DE {_limitePassos} PASSOS ATINGIDO");
                return false;
            }

            char simboloAtual = fita.GetValueOrDefault(cabecote, Branco);

            // ausência de transição para (q, s) equivale a rejeição implícita
            if (!_delta.TryGetValue((estadoAtual, simboloAtual), out var transicao))
            {
                Console.WriteLine(
                    $"  Resultado : REJEITA (transição δ({estadoAtual}, '{simboloAtual}') indefinida)"
                );
                return false;
            }

            fita[cabecote] = transicao.novoSimbolo;
            estadoAtual = transicao.novoEstado;
            cabecote += transicao.direcao == 'R' ? 1 : -1;
            passos++;
        }

        bool aceita = estadoAtual == _qaccept;
        Console.WriteLine($"  Resultado : {(aceita ? "ACEITA" : "REJEITA")}");
        return aceita;
    }

    private static void ExibirPasso(
        int passo,
        string estado,
        Dictionary<int, char> fita,
        int cabecote
    )
    {
        string conteudoFita = MontarFita(fita, cabecote);
        Console.WriteLine(
            $"  Passo {passo, 4}: estado={estado, -10} fita={conteudoFita, -40} cabeçote={cabecote}"
        );
    }

    private static string MontarFita(Dictionary<int, char> fita, int cabecote)
    {
        if (fita.Count == 0 && cabecote == 0)
            return $"[{Branco}]";

        int min = Math.Min(fita.Keys.DefaultIfEmpty(0).Min(), cabecote);
        int max = Math.Max(fita.Keys.DefaultIfEmpty(0).Max(), cabecote);

        var partes = new List<string>();
        for (int i = min; i <= max; i++)
        {
            char c = fita.GetValueOrDefault(i, Branco);
            partes.Add(i == cabecote ? $"[{c}]" : c.ToString());
        }
        return string.Join(" ", partes);
    }

    public void ExibirDiagrama()
    {
        var estados = _q.OrderBy(e => e).ToList();
        var simbolos = _gamma.OrderBy(s => s).ToList();

        // largura de célula: comporta "(novoEstado, s, D)" — usa o maior nome de estado
        int maxEstado = _q.Max(e => e.Length);
        int larguraCelula = Math.Max(maxEstado + 5, 12);
        int larguraEstado = estados.Max(e => e.Length) + 5;

        Console.WriteLine();
        Console.WriteLine("=== Tabela de Transições ===");

        string cabecalho =
            "Estado".PadRight(larguraEstado)
            + "| "
            + string.Join(" | ", simbolos.Select(s => s.ToString().PadRight(larguraCelula)));
        Console.WriteLine(cabecalho);
        Console.WriteLine(
            new string('-', larguraEstado)
                + "+"
                + string.Join("-+-", simbolos.Select(_ => new string('-', larguraCelula + 1)))
        );

        foreach (string estado in estados)
        {
            // prefixo: → estado inicial, * estado de aceitação
            string marcador = (estado == _q0 ? "→ " : "  ") + (estado == _qaccept ? "* " : "  ");
            string linha =
                (marcador + estado).PadRight(larguraEstado)
                + "| "
                + string.Join(
                    " | ",
                    simbolos.Select(s =>
                        _delta.TryGetValue((estado, s), out var t)
                            ? $"({t.novoEstado},{t.novoSimbolo},{t.direcao})".PadRight(
                                larguraCelula
                            )
                            : "-".PadRight(larguraCelula)
                    )
                );
            Console.WriteLine(linha);
        }

        Console.WriteLine();
        Console.WriteLine("  →  = estado inicial     *  = estado de aceitação");
    }
}
