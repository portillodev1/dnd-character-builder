# HARNESS.md — Manifesto de Harness

> Manifesto vivo. Atualize ao adicionar/alterar controle ou aplicar steering.
> Inventário completo: [docs/harness/controls-inventory.md](docs/harness/controls-inventory.md).

## 1. Harnessability
- **Tipagem**: C# (nullable + warnings-as-errors) e TypeScript strict → type-check é sensor grátis.
- **Fronteiras**: claras por projeto .NET (`Domain` puro) e por feature Angular; testáveis por reflexão/ESLint.
- **Frameworks fortes**: ASP.NET Core Minimal APIs, Angular 22 (standalone, signals). Pouca superfície livre.
- **Situação**: greenfield, **sem código**. Harness assado desde o dia 1; sensores de código
  ficam "planejados" e viram obrigatórios com `verify.sh --strict` na Fase 1 do `ROADMAP.md`.
- **Risco maior**: regra de D&D errada (comportamento) e número sem explicação (critério 4).

## 2. Controles por categoria (resumo)

### Manutenibilidade
| Controle | Direção | Execução | Onde roda | Status |
|---|---|---|---|---|
| `AGENTS.md` + `rules/*.md` (progressive loading) | guia | inferencial | antes da geração | ✅ |
| `scripts/check-docs.sh` (md ≤ 100 linhas) | sensor | computacional | verify | ✅ |
| `dotnet format --verify-no-changes` / `ng lint` + Prettier | sensor | computacional | verify | planejado |
| Compilador C# (warnings-as-errors) / `tsc` strict | sensor | computacional | build | planejado |

### Fitness de arquitetura
| Controle | Direção | Execução | Onde roda | Status |
|---|---|---|---|---|
| `rules/backend-dotnet.md`, `rules/frontend-angular.md`, ADRs | guia | inferencial | antes da geração | ✅ |
| Teste de dependência: `Domain` sem referências | sensor | computacional | test | planejado |
| ESLint: proibir import entre `features/*` e fórmula no front | sensor | computacional | lint | planejado |
| Startup falha se `content/` inválido | sensor | computacional | runtime/test | planejado |

### Comportamento
| Controle | Direção | Execução | Onde roda | Status |
|---|---|---|---|---|
| Specs `.specs/features/*/spec.md` + `docs/domain/calculations.md` | guia | inferencial | antes da geração | ✅ |
| `Domain.Tests` — testes de tabela por fórmula (LdJ) | sensor | computacional | test | planejado |
| Invariante `Total == ΣParts` em todo `DerivedValue` | sensor | computacional | test | planejado |
| `Ruleset.Tests` — schema + integridade do catálogo | sensor | computacional | test | planejado |
| `Api.Tests` — contrato HTTP / ProblemDetails | sensor | computacional | test | planejado |
| Playwright — wizard ponta a ponta (fase 2) | sensor | computacional | pipeline | planejado |
| Review agent contra spec (só no fim de cada feature) | sensor | inferencial | pré-integração | planejado |

## 3. Posicionamento no ciclo ("keep quality left")
- **Pré-geração**: AGENTS.md → rule da tarefa → spec da feature.
- **Pré-commit / fim de tarefa**: `scripts/verify.sh` (check-docs, build, lint, test). Obrigatório.
- **Pipeline (fase 1)**: GitHub Actions rodando `scripts/verify.sh --strict` em PR.
- **Contínuo**: `check-docs.sh` contra inchaço de docs; revisão de `content/` contra LdJ por humano.

## 4. Loop de autocorreção
- **Eval gates (0/1) expostos ao `loop-engineering`**:
  1. `scripts/check-docs.sh`
  2. `scripts/build.sh`
  3. `scripts/lint.sh`
  4. `scripts/test.sh --back` (Domain, Ruleset, Api)
  5. `scripts/test.sh --front`
  6. Agregado: `scripts/verify.sh --strict`
- Sinal para o LLM: scripts imprimem `FAIL: <arquivo> — <o que fazer>`; testes de domínio
  nomeiam a regra e a página do LdJ.
- **Contrato implementador × validador**: o implementador lista os critérios de aceite da
  spec que vai cobrir; o validador confere a lista contra a spec e testa item a item
  (mecânica do contrato é da skill `loop-engineering`).

## 5. Memória e bootstrap
- **Estado**: `STATE.md` (decisões `AD-NNN` + `## Handoff`). **Roadmap**: `ROADMAP.md`.
- **Bootstrap**: `scripts/bootstrap.sh` imprime handoff, git recente, toolchain e status do scaffold.
- **Git**: 1 tarefa = 1 commit (após aprovação), mensagem cita o critério da spec.

## 6. Steering log
| Data | Problema recorrente | Controle criado/ajustado |
|---|---|---|
| 2026-10-19 | (setup) docs gerados sem teto de tamanho tendem a inchar | `check-docs.sh` no gate |
