# Inventário de controles do harness

Classificação de cada controle existente e planejado. Buraco = célula vazia numa
categoria que importa. Manifesto resumido: `HARNESS.md`.

**Legenda** — Direção: guia | sensor · Execução: comp. (computacional) | inf. (inferencial)
· Estágio: pré-geração | pré-commit | pipeline | contínuo.

| Controle | Direção | Exec. | Categoria | Estágio | Sinal p/ LLM | Status |
|---|---|---|---|---|---|---|
| `AGENTS.md` (índice, boundaries) | guia | inf. | manutenibilidade | pré-geração | — | ✅ |
| `rules/*.md` sob demanda | guia | inf. | manutenib./arquitetura | pré-geração | — | ✅ |
| ADRs `docs/adr/` | guia | inf. | arquitetura | pré-geração | — | ✅ |
| `docs/domain/calculations.md` | guia | inf. | comportamento | pré-geração | — | ✅ |
| Specs `.specs/` | guia | inf. | comportamento | pré-geração | — | ✅ |
| `check-docs.sh` (≤ 100 linhas) | sensor | comp. | manutenibilidade | pré-commit | ✅ arquivo + ação | ✅ |
| `dotnet format` | sensor | comp. | manutenibilidade | pré-commit | ✅ | ✅ |
| `ng lint` (angular-eslint) + Prettier | sensor | comp. | manutenibilidade | pré-commit | ✅ | ✅ |
| Compilador C# warnings-as-errors | sensor | comp. | manutenibilidade | pré-commit | ✅ | ✅ |
| `tsc` strict + templates strict | sensor | comp. | manutenibilidade | pré-commit | ✅ | ✅ |
| Teste de dependência do `Domain` | sensor | comp. | arquitetura | pré-commit | ✅ mensagem custom | ✅ |
| ESLint boundaries entre features | sensor | comp. | arquitetura | pré-commit | ✅ mensagem custom | ✅ |
| Tipos do front gerados do OpenAPI (diff = falha) | sensor | comp. | arquitetura | pipeline | ☐ | ☐ |
| `Domain.Tests` (tabela, LdJ) | sensor | comp. | comportamento | pré-commit | ☐ regra + página | ☐ |
| Invariante `Total == ΣParts` | sensor | comp. | comportamento | pré-commit | ☐ | ☐ |
| `Ruleset.Tests` (schema + integridade) | sensor | comp. | comportamento | pré-commit | ☐ caminho JSON | ☐ |
| `Api.Tests` (WebApplicationFactory) | sensor | comp. | comportamento | pré-commit | ☐ | ☐ |
| Vitest (componentes/store) | sensor | comp. | comportamento | pré-commit | ☐ | ☐ |
| Cobertura `Domain` ≥ 90% | sensor | comp. | comportamento | pipeline | ☐ | ☐ |
| Playwright wizard E2E | sensor | comp. | comportamento | pipeline | ☐ | ☐ (fase 2) |
| axe (acessibilidade) no E2E | sensor | comp. | comportamento | pipeline | ☐ | ☐ (fase 2) |
| Review agent vs. spec | sensor | inf. | comportamento | pré-integração | — | ☐ |
| Revisão humana de `content/` vs. LdJ | sensor | inf. | comportamento | pré-integração | — | ☐ |
| CI GitHub Actions `verify.sh --strict` | sensor | comp. | todas | pipeline | ✅ | ✅ |
| Mutation testing (Stryker.NET) no `Domain` | sensor | comp. | comportamento | contínuo | ☐ | ☐ (opcional) |

## Buracos conhecidos (aceitos por ora)
- Sem teste de performance: API stateless e catálogo em memória; risco baixo.
- Sem observabilidade além de logs padrão do ASP.NET: MVP local.
