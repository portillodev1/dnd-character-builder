# rules/tests.md — carregada em: escrever/ajustar testes

O agente **não se autoavalia**: quem decide "pronto" é o gate (`scripts/verify.sh`).

## Regras gerais
- Todo comportamento novo/alterado tem teste antes de concluir.
- Nomeie por comportamento: `<Unidade>_<Condição>_<Resultado>` (C#) /
  `'<unidade> quando <condição> deve <resultado>'` (TS).
- Um assert lógico por teste; sem rede/relógio/disco reais — use fakes/fixtures.
- Bug corrigido entra com teste de regressão que falha sem o fix.
- Rode via `scripts/test.sh` (`--back`, `--front`, `--filter`). Nunca monte o runner na mão.

## Pirâmide por camada
| Camada | Ferramenta | O que cobre |
|---|---|---|
| `Domain.Tests` | xUnit v3 | Fórmulas (mod., proficiência, perícias, salvaguardas, breakdown) — **maior volume** |
| `Ruleset.Tests` | xUnit v3 | Todo JSON de `content/` valida no schema e tem `source` |
| `Api.Tests` | xUnit v3 + `WebApplicationFactory` | Contrato HTTP, status codes, ProblemDetails |
| `frontend` unit | Vitest (runner padrão Angular) | Componentes/stores sem regra de D&D |
| E2E (`frontend/e2e/`) | Playwright + axe | Critérios F1–F5 ponta a ponta e WCAG 2.2 AA — `scripts/e2e.sh` |

## Regras de domínio = testes de tabela
- Fórmulas do Livro do Jogador são testadas com **casos de tabela** (Theory/InlineData)
  cobrindo limites: valor 1, 10, 11, 20, 30; níveis 1, 4, 5, 16, 17, 20.
- Cada teste de regra cita a página no nome do caso ou comentário (`// LdJ p. 175`).

## Exemplo
```csharp
[Theory]
[InlineData(1, 2)] [InlineData(4, 2)] [InlineData(5, 3)] [InlineData(17, 6)]
public void ProficiencyBonus_ForLevel_MatchesTable(int level, int expected) // LdJ p. 15
    => Assert.Equal(expected, ProficiencyBonus.ForLevel(level).Value);
```
