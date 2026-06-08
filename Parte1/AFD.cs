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
                new()
                {
                    Origem = "q0",
                    Simbolo = "a",
                    Destino = "q1",
                },
                new()
                {
                    Origem = "q0",
                    Simbolo = "b",
                    Destino = "q0",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "a",
                    Destino = "q1",
                },
                new()
                {
                    Origem = "q1",
                    Simbolo = "b",
                    Destino = "q2",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "a",
                    Destino = "q1",
                },
                new()
                {
                    Origem = "q2",
                    Simbolo = "b",
                    Destino = "q0",
                },
            ],
        };
}
