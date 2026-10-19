# D&D Character Builder

Ferramenta para ajudar jogadores iniciantes de D&D 5e a criar a ficha de personagem,
explicando **de onde vem cada número**: traços da raça, características da classe por
nível, perícias, salvaguardas e o tipo de cada bônus.

## Status
**Fase 0 — Fundação**: arquitetura, harness e specs prontos; ainda sem código.
Próximos passos em [ROADMAP.md](ROADMAP.md).

## Stack
Angular 22 (Node 24 LTS) · .NET 10 LTS (ASP.NET Core Minimal APIs) · catálogo de regras em JSON.

## Onde começar
| Você é… | Leia |
|---|---|
| Pessoa desenvolvedora | [docs/README.md](docs/README.md) → arquitetura e domínio |
| Agente de código | [AGENTS.md](AGENTS.md) |
| Quer ver o escopo | [.specs/project/spec.md](.specs/project/spec.md) e [ROADMAP.md](ROADMAP.md) |

## Pré-requisitos (a partir da Fase 1)
.NET SDK 10 · Node 24 LTS · Git Bash (Windows). Gate: `bash scripts/verify.sh --strict`.

## Referência e licença do conteúdo
Regras baseadas no Livro do Jogador de D&D 5e (páginas citadas para consulta) e no
System Reference Document 5.1 da Wizards of the Coast, licenciado sob CC-BY-4.0.
Ver [ADR-0004](docs/adr/ADR-0004-content-licensing.md).
