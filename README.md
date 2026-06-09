# ftc-20261-final
Criado para o trabalho final de FTC: Implementação de Máquinas Abstratas: AFD, Autômato de Pilha e Máquina de Turing

---

## Desenvolvedores

| Matrícula | Nome |
|-----------|------|
| 72400676 | Lucas Alves Souza Lima |
| 72400625 | João Gabriel Vieira Valério Flor |

---

## Estrutura do Projeto

```
ftc-20261-final/
├── Parte1/   — Autômato Finito Determinístico (AFD)
├── Parte2/   — Autômato de Pilha (AP)
├── Parte3/   — Máquina de Turing (MT)
└── docs/     — TODO
```

---

## Parte 1 — Autômato Finito Determinístico (AFD)

**Linguagem:** L1 = { w ∈ {a,b}* | w termina com "ab" }

### Da teoria ao código

A definição formal do AFD é a 5-tupla **M = (Q, Σ, δ, q0, F)**. Cada componente foi mapeado diretamente para campos da classe `AFD`:

| Componente | Representação no código |
|------------|------------------------|
| Q — conjunto de estados | `HashSet<string> _q` |
| Σ — alfabeto de entrada | `HashSet<char> _sigma` |
| δ — função de transição | `Dictionary<(estado, simbolo), estadoDestino> _delta` |
| q0 — estado inicial | `string _q0` |
| F — estados de aceitação | `HashSet<string> _f` |

O método `Aceitar(string cadeia)` implementa a função de transição estendida δ*: começa em q0 e, para cada símbolo lido, consulta o dicionário `_delta[(estadoAtual, c)]`. Ao final da cadeia, verifica se o estado atual pertence a F. A ausência de uma entrada no dicionário representa um símbolo fora do alfabeto — rejeição imediata.

A configuração pode ser carregada de um arquivo `afd.json` (via `AFDLoader`) ou usa a configuração padrão de L1 embutida na própria classe.

### Como executar

```bash
cd Parte1
dotnet run
```

---

## Parte 2 — Autômato de Pilha (AP)

**Linguagens:**
- L2 = { aⁿbⁿ | n ≥ 1 }
- L3 = { w ∈ {a,b}* | w = wᴿ, |w| ≥ 1 } (palíndromos)

### Da teoria ao código

O AP é definido pela 7-tupla **M = (Q, Σ, Γ, δ, q0, Z0, F)**. A transição δ mapeia `(estado, símbolo, topo da pilha)` para um conjunto de pares `(estado destino, string a empilhar)`, modelando o não-determinismo:

```
Dictionary<(estado, símbolo, topoPilha), List<(estadoDestino, empilhar)>> _delta
```

A aceitação é por **pilha vazia** (F = ∅): a cadeia é aceita quando toda a entrada foi consumida e a pilha está vazia.

O método `Simular` implementa a busca em profundidade com backtracking para explorar todos os caminhos não-determinísticos. A cada passo, tenta primeiro transições que consomem um símbolo da entrada e depois λ-movimentos (`'\0'`). A pilha é clonada a cada ramificação para que o backtracking seja correto.

Para L3 (palíndromos), o AP é não-determinístico: em `q0` ele tanto empilha símbolos quanto pode transitar não-deterministicamente para `q1` (ponto médio), onde começa a desempilhar e comparar com a segunda metade da cadeia.

### Como executar

```bash
cd Parte2
dotnet run
```

---

## Parte 3 — Máquina de Turing (MT)

**Linguagens / funções:**
- L4 = { aⁿbⁿcⁿ | n ≥ 1 }
- Função unária: f(n) = n + 1 (incremento em unário)

### Da teoria ao código

A MT é definida pela 7-tupla **M = (Q, Σ, Γ, δ, q0, qaccept, qreject)**. A fita é modelada como um dicionário esparso — posições não escritas retornam o símbolo branco `'_'` — o que simula a fita infinita da teoria sem alocar memória desnecessária:

```
Dictionary<int, char> fita   // fita[i] = símbolo na célula i; ausente → '_'
```

A função de transição δ(estado, símbolo) → (novoEstado, novoSímbolo, direção) é armazenada como:

```
Dictionary<(estado, símbolo), (novoEstado, novoSímbolo, direção)> _delta
```

O método `Aceitar` executa o loop de simulação passo a passo: lê o símbolo sob o cabeçote, consulta `_delta`, escreve o novo símbolo, muda de estado e move o cabeçote (R = +1, L = −1). O loop termina ao atingir qaccept ou qreject, ou ao exceder o limite de passos configurado.

Para L4, a MT usa marcação iterativa: substitui `a→A`, `b→B`, `c→C` em cada passada da esquerda para a direita, garantindo que os três contadores sejam iguais. A função unária simplesmente varre todos os `1`s da fita e escreve um `1` adicional na primeira célula em branco.

A configuração pode ser carregada de arquivos JSON (`mt_l4.json`, `mt_unario.json`) via `MTLoader`.

### Como executar

```bash
cd Parte3
dotnet run
```

---

## Video explicativo

[![Assista no YouTube](https://img.youtube.com/vi/dQw4w9WgXcQ/0.jpg)](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
