# AGENTS.md

> Índice/roteador para agentes de código — **não é manual**. Regra detalhada mora em
> `rules/` (carregue sob demanda). `docs/` é para **humanos**: referencie, não copie.
> Ative cada skill **pelo nome**.

## BOUNDARIES (prioridade máxima — leia sempre)

**SEMPRE FAÇA**
- **PROGRESSIVE LOADING — NÃO LEIA TUDO.** Abra só o que a Quick reference indica para o pedido atual.
- Mantenha cada doc/rule/spec **30–70 linhas (ideal), ≤100 (teto)**. Passou → quebre em `.md` menores + índice.
- Rode tarefas repetíveis via `scripts/` (ex.: `scripts/test.sh`), nunca montando `dotnet`/`ng`/`npx` na mão.
- Antes de dizer "pronto", rode `scripts/verify.sh` e só conclua se sair 0.
- Toda regra de D&D implementada cita a página do Livro do Jogador (ver `docs/domain/README.md`).
- Comece rodando `scripts/bootstrap.sh` (handoff do `STATE.md`); atualize `## Handoff` ao terminar uma unidade.

**PERGUNTE QUANDO**
- For commitar (padrão: **NÃO** commitar sem aprovação).
- For apagar/renomear spec, design ou doc; ou mudar contrato público da API (`docs/architecture/api-contract.md`).
- Precisar de dependência nova ou decisão de arquitetura → registre ADR em `docs/adr/`.
- For incluir conteúdo de regras **fora do SRD 5.1** no catálogo (ver ADR-0004).

**NUNCA FAÇA**
- **LER TUDO de uma vez** (docs/rules/specs ou o repo inteiro).
- Gerar doc/rule/spec com mais de 100 linhas.
- Commitar sem aprovação explícita.
- Versionar o PDF do Livro do Jogador ou colar trechos longos dele no repo (ADR-0004).
- Colocar regra de cálculo de D&D no front-end — cálculo mora no `Domain` do back-end (ADR-0002).
- Editar código gerado ou segredos (`.env`, chaves).

## Projeto
- **Propósito**: ajudar jogadores iniciantes de D&D 5e a montar a ficha entendendo *de onde vem cada número*.
- **Alvo**: web app (SPA) + API HTTP. Sem login nem banco no MVP (ADR-0002).
- **Stack**: Angular 22 + Node 24 LTS (`frontend/`) · .NET 10 LTS / ASP.NET Core (`backend/`).
- **Arquitetura**: monorepo; API stateless com motor de regras puro; catálogo de regras em JSON versionado (`content/`).

## Comandos → use os scripts
| Preciso… | Rode | Em vez de |
|---|---|---|
| Testar | `scripts/test.sh [--front\|--back] [--filter <p>]` | `dotnet test` / `ng test` na mão |
| Lint/format | `scripts/lint.sh [--fix]` | lembrar flags do linter |
| Build | `scripts/build.sh` | — |
| Gate final | `scripts/verify.sh [--strict]` | concluir sem validar |

## Quick reference — o que carregar por tipo de tarefa
> **Carregue SÓ a linha que casa com o pedido atual.**

| Tarefa | Carregue | Skill p/ acionar |
|---|---|---|
| Nova feature / fluxo | `rules/implementation.md` + spec da feature em `.specs/` | `spec-driven-development` |
| Código .NET (API, Domain) | `rules/backend-dotnet.md` | — |
| Código Angular | `rules/frontend-angular.md` | — |
| Dados de raça/classe (`content/`) | `rules/ruleset-content.md` | — |
| Fórmulas (mod., proficiência, perícias) | `docs/domain/calculations.md` | — |
| Escrever/ajustar testes | `rules/tests.md` | `harness-engineering` |
| Visão de arquitetura | `docs/architecture/README.md` | — |
| Rodar autônomo até passar | `HARNESS.md` | `loop-engineering` (só se necessário) |
| Rotear a demanda | `STATE.md` | `agentic-delivery` |
| API/versão atual de lib | **context7 MCP** (cite a versão) | — |

## Regras gerais (resumo — detalhe em `rules/`)
- **Implementação**: 1 comportamento por tarefa; siga os padrões do código; sem one-shot hero.
- **Testes**: todo comportamento novo tem teste; o agente não se autoavalia — quem decide é o gate.
- **Conteúdo**: catálogo validado por schema + teste; toda entrada tem `source` com página.

## Harness e estado
- Manifesto: `HARNESS.md` · Estado/handoff: `STATE.md` · Roadmap: `ROADMAP.md`.
- Specs: `.specs/` (skill `spec-driven-development`).

## Política de commit e limpeza
- **Commit sob aprovação** (padrão: não commitar).
- **Limpeza após aprovação**: remova efêmeros (`tasks.md`, logs, contratos de iteração).
  Preserve `spec.md`, `design.md`, `AGENTS.md`, `rules/`, `docs/`, `HARNESS.md`, `STATE.md`, `ROADMAP.md`.
- **Loop**: não limpe a cada tarefa; só quando o loop inteiro terminar e for aprovado.

## Mapa de arquivos
| Caminho | Papel | Público | Ciclo |
|---|---|---|---|
| `AGENTS.md` / `rules/*.md` | Índice + regras testáveis | IA | permanente |
| `scripts/*` | Comandos executáveis (gates) | IA | permanente |
| `docs/**`, `docs/adr/*` | Contexto e decisões | Humano | permanente |
| `.specs/**/spec.md`, `design.md` | Especificação | IA+Humano | permanente |
| `.specs/**/tasks.md`, logs | Trabalho de 1 tarefa | IA | efêmero |
