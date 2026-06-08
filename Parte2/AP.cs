namespace Parte2;

// Representação da 7-tupla M = (Q, Σ, Γ, δ, q0, Z0, F)
class AP
{
    private readonly HashSet<string> _q;
    private readonly HashSet<char> _sigma;
    private readonly HashSet<char> _gamma;

    // δ: Q × (Σ ∪ {λ}) × Γ → P(Q × Γ*)
    private readonly Dictionary<
        (string estado, char simbolo, char topoPilha),
        List<(string estadoDestino, string empilhar)>
    > _delta;
    private readonly string _q0;
    private readonly char _z0;
    private readonly HashSet<string> _f;

    public AP(
        IEnumerable<string> q,
        IEnumerable<char> sigma,
        IEnumerable<char> gamma,
        string q0,
        char z0,
        IEnumerable<string> f
    )
    {
        _q = new HashSet<string>(q);
        _sigma = new HashSet<char>(sigma);
        _gamma = new HashSet<char>(gamma);
        _delta = new Dictionary<(string, char, char), List<(string, string)>>();
        _q0 = q0;
        _z0 = z0;
        _f = new HashSet<string>(f);
    }

    public void AdicionarTransicao(
        string estado,
        char simbolo,
        char topoPilha,
        string estadoDestino,
        string empilhar
    )
    {
        var chave = (estado, simbolo, topoPilha);
        if (!_delta.ContainsKey(chave))
        {
            _delta[chave] = new List<(string, string)>();
        }
        _delta[chave].Add((estadoDestino, empilhar));
    }

    public bool Aceitar(string cadeia)
    {
        var pilhaInicial = new Stack<char>();
        pilhaInicial.Push(_z0);

        var rastroSucesso = new List<string>();

        bool aceitou = Simular(cadeia, 0, _q0, pilhaInicial, new List<string>(), rastroSucesso);

        if (aceitou)
        {
            Console.WriteLine("  Caminho de Aceitação:");
            foreach (var passo in rastroSucesso)
            {
                Console.WriteLine($"    {passo}");
            }
            Console.WriteLine("  Resultado : ACEITA");
            return true;
        }
        else
        {
            Console.WriteLine("  Resultado : REJEITA");
            return false;
        }
    }

    private bool Simular(
        string cadeia,
        int indice,
        string estadoAtual,
        Stack<char> pilhaAtual,
        List<string> rastroAtual,
        List<string> rastroSucesso
    )
    {
        //Guarda estado atual
        string restante = indice < cadeia.Length ? cadeia.Substring(indice) : "ε";
        string conteudoPilha = pilhaAtual.Count > 0 ? string.Join("", pilhaAtual.ToArray()) : "ε";
        string passo = $"(estado: {estadoAtual}, restante: {restante}, pilha: [{conteudoPilha}])";

        rastroAtual.Add(passo);

        //Verifica se a cadeia foi totalmente lida e pilha vazia
        if (indice == cadeia.Length && pilhaAtual.Count == 0)
        {
            rastroSucesso.AddRange(rastroAtual);
            return true;
        }

        // Se a pilha está vazia mas não terminamos de ler, falha neste caminho
        if (pilhaAtual.Count == 0)
        {
            rastroAtual.RemoveAt(rastroAtual.Count - 1);
            return false;
        }

        char topo = pilhaAtual.Peek();

        // Tentar transições que consomem um símbolo da cadeia (se houver)
        if (indice < cadeia.Length)
        {
            char simbolo = cadeia[indice];
            if (_delta.TryGetValue((estadoAtual, simbolo, topo), out var transicoesSimbolo))
            {
                foreach (var (estadoDestino, empilhar) in transicoesSimbolo)
                {
                    var novaPilha = ClonarPilhaEPush(pilhaAtual, empilhar);
                    if (
                        Simular(
                            cadeia,
                            indice + 1,
                            estadoDestino,
                            novaPilha,
                            rastroAtual,
                            rastroSucesso
                        )
                    )
                    {
                        return true;
                    }
                }
            }
        }

        // Tentar λ-movimentos ('\0')
        if (_delta.TryGetValue((estadoAtual, '\0', topo), out var transicoesLambda))
        {
            foreach (var (estadoDestino, empilhar) in transicoesLambda)
            {
                var novaPilha = ClonarPilhaEPush(pilhaAtual, empilhar);
                if (Simular(cadeia, indice, estadoDestino, novaPilha, rastroAtual, rastroSucesso))
                {
                    return true;
                }
            }
        }

        rastroAtual.RemoveAt(rastroAtual.Count - 1);
        return false;
    }

    private Stack<char> ClonarPilhaEPush(Stack<char> pilhaOriginal, string empilhar)
    {
        var array = pilhaOriginal.ToArray();
        var novaPilha = new Stack<char>();

        // Inserimos de trás para frente para manter a ordem da pilha original
        for (int i = array.Length - 1; i >= 0; i--)
        {
            novaPilha.Push(array[i]);
        }

        // Remove o topo original, pois ele foi consumido pela transição
        novaPilha.Pop();

        // Empilha a string de trás para frente
        for (int i = empilhar.Length - 1; i >= 0; i--)
        {
            novaPilha.Push(empilhar[i]);
        }

        return novaPilha;
    }
}
