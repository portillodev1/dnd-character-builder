# ROADMAP

Objetivo: iniciante monta a ficha de D&D 5e entendendo de onde vem cada número.
**Pronto (MVP)** = critérios 1–4 abaixo atendidos e `scripts/verify.sh --strict` verde.

| # | Critério de pronto | Feature (spec) |
|---|---|---|
| 1 | Vê traços/particularidades da raça (e sub-raça) escolhida | [F1 race-traits](.specs/features/race-traits/spec.md) |
| 2 | Vê características da classe até o nível do personagem | [F2 class-features](.specs/features/class-features/spec.md) |
| 3 | Sabe calcular e preencher perícias e salvaguardas | [F3 skills-saves](.specs/features/skills-saves/spec.md) |
| 4 | Entende cada bônus (raça/classe) e o **tipo** dele | [F4 bonus-breakdown](.specs/features/bonus-breakdown/spec.md) |

## Fases
| Fase | Entrega | Gate de saída | Status |
|---|---|---|---|
| 0 — Fundação | AGENTS.md, rules, scripts, harness, arquitetura, specs | `verify.sh` verde (SKIP permitido) | ✅ aguardando revisão |
| 1 — Scaffold | .NET 10 SDK; `DndBuilder.slnx` + 4 projetos + 3 de teste; `ng new` Angular 22; ESLint/Prettier; CI | `verify.sh --strict` verde com 1 teste trivial por projeto | ☐ |
| 2 — Motor de regras | `Domain`: atributos, modificador, proficiência, perícias, salvaguardas, `DerivedValue` | `Domain.Tests` cobre `docs/domain/calculations.md` §1–6 | ☐ |
| 3 — Catálogo | schemas + 9 raças (+ sub-raças) + 12 classes (níveis 1–20, classe base) | `Ruleset.Tests` verde; revisão humana vs. LdJ | ☐ |
| 4 — API | endpoints de `docs/architecture/api-contract.md` + OpenAPI | `Api.Tests` verde | ☐ |
| 5 — Wizard | passos Raça → Classe → Atributos → Perícias/Salvaguardas → Ficha | Vitest verde; critérios F1–F4 demonstráveis | ☐ |
| 6 — Qualidade | Playwright E2E, axe, polimento de textos | E2E dos 4 critérios verde | ☐ |

Dependências: 1 → 2 → (3 ∥ 4 parcialmente) → 5 → 6. Fases 2 e 3 podem correr em paralelo
depois que o schema (`content/schemas/`) estiver fechado.

## Pós-MVP (backlog, cada item exige spec)
- Antecedentes completos (Cap. 4) — hoje só proficiências manuais extras.
- Arquétipos/subclasses detalhados (depende de ADR-0004).
- Aumento de atributo por nível (ASI) e talentos.
- Magias por classe/nível; equipamento e CA calculada.
- Salvar/compartilhar ficha; exportar PDF no layout oficial.
