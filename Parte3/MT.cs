namespace Parte3;

// Modela a 7-tupla M = (Q, Σ, Γ, δ, q0, qaccept, qreject)
class MT
{
    private readonly HashSet<string> _q;
    private readonly HashSet<char> _sigma;
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

    public IReadOnlySet<string> Q => _q;
    public IReadOnlySet<char> Sigma => _sigma;
    public IReadOnlySet<char> Gamma => _gamma;
    public IReadOnlyDictionary<
        (string estado, char simbolo),
        (string novoEstado, char novoSimbolo, char direcao)
    > Delta => _delta;
    public string Q0 => _q0;
    public string Qaccept => _qaccept;
    public string Qreject => _qreject;
    public int LimitePassos => _limitePassos;

    // Sem configuração → fallback L4 = { aⁿbⁿcⁿ | n ≥ 1 }
    public MT()
        : this(ConfiguracaoL4Padrao()) { }

    public MT(MTConfig config)
    {
        _q = new HashSet<string>(config.Estados);
        _sigma = new HashSet<char>(
            config.AlfabetoEntrada.Where(s => s.Length == 1).Select(s => s[0])
        );
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
                new()
                {
                    Origem = "q0",
                    Simbolo = "A",
                    NovoEstado = "q0",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "a",
                    NovoEstado = "q1",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "B",
                    NovoEstado = "q4",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "b",
                    NovoEstado = "qreject",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "C",
                    NovoEstado = "qreject",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                // q1: marcou a→A; varre à direita procurando 'b' não marcado
                new()
                {
                    Origem = "q1",
                    Simbolo = "a",
                    NovoEstado = "q1",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "B",
                    NovoEstado = "q1",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "b",
                    NovoEstado = "q2",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "C",
                    NovoEstado = "qreject",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q2: marcou b→B; varre à direita procurando 'c' não marcado
                new()
                {
                    Origem = "q2",
                    Simbolo = "b",
                    NovoEstado = "q2",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "C",
                    NovoEstado = "q2",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "c",
                    NovoEstado = "q3",
                    NovoSimbolo = "C",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "a",
                    NovoEstado = "qreject",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "A",
                    NovoEstado = "qreject",
                    NovoSimbolo = "A",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "_",
                    NovoEstado = "qreject",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q3: marcou c→C; varre à esquerda de volta ao branco inicial
                new()
                {
                    Origem = "q3",
                    Simbolo = "C",
                    NovoEstado = "q3",
                    NovoSimbolo = "C",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "c",
                    NovoEstado = "q3",
                    NovoSimbolo = "c",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "B",
                    NovoEstado = "q3",
                    NovoSimbolo = "B",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "b",
                    NovoEstado = "q3",
                    NovoSimbolo = "b",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "A",
                    NovoEstado = "q3",
                    NovoSimbolo = "A",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "a",
                    NovoEstado = "q3",
                    NovoSimbolo = "a",
                    Direcao = "L",
                },
                new()
                {
                    Origem = "q3",
                    Simbolo = "_",
                    NovoEstado = "q0",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                // q4: sem mais a's; verifica que só restam B's, C's e branco
                new()
                {
                    Origem = "q4",
                    Simbolo = "B",
                    NovoEstado = "q4",
                    NovoSimbolo = "B",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q4",
                    Simbolo = "C",
                    NovoEstado = "q4",
                    NovoSimbolo = "C",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q4",
                    Simbolo = "_",
                    NovoEstado = "qaccept",
                    NovoSimbolo = "_",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q4",
                    Simbolo = "b",
                    NovoEstado = "qreject",
                    NovoSimbolo = "b",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q4",
                    Simbolo = "c",
                    NovoEstado = "qreject",
                    NovoSimbolo = "c",
                    Direcao = "R",
                },
                new()
                {
                    Origem = "q4",
                    Simbolo = "a",
                    NovoEstado = "qreject",
                    NovoSimbolo = "a",
                    Direcao = "R",
                },
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
        // TODO: Imprimir a tabela de transições δ formatada como ASCII.
        //
        // 1. Ordenar _q (estados) e _gamma (símbolos de fita) alfabeticamente.
        //
        // 2. Calcular larguras de coluna:
        //    - larguraCelula = max(tamanho do maior nome de estado + 5, 12)
        //    - larguraEstado = tamanho do maior nome de estado + 5
        //
        // 3. Imprimir cabeçalho:
        //    - Linha: "Estado" preenchido à direita até larguraEstado, depois "| " e cada símbolo
        //      separado por " | ", cada um preenchido até larguraCelula.
        //    - Separador: traços de larguraEstado + "+" + traços intercalados com "-+-".
        //
        // 4. Para cada estado, imprimir uma linha com:
        //    - Marcador "→ " se é _q0, "  " caso contrário; "* " se é _qaccept, "  " caso contrário.
        //    - Nome do estado preenchido até larguraEstado.
        //    - Para cada símbolo: se existe transição em _delta, exibir "(novoEstado,novoSimbolo,direcao)"
        //      preenchido até larguraCelula; senão exibir "-" preenchido até larguraCelula.
        //    - Colunas separadas por " | ".
        //
        // 5. Imprimir legenda: "  →  = estado inicial     *  = estado de aceitação"
        throw new NotImplementedException("TODO: ExibirDiagrama");
    }
}
