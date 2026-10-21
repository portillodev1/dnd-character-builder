# ROADMAP

Objetivo: iniciante monta a ficha de D&D 5e entendendo de onde vem cada número.
**Pronto (MVP)** = critérios 1–5 abaixo atendidos e `scripts/verify.sh --strict` verde.

| # | Critério de pronto | Feature (spec) |
|---|---|---|
| 1 | Vê traços/particularidades da raça (e sub-raça) escolhida | [F1 race-traits](.specs/features/race-traits/spec.md) |
| 2 | Vê características da classe até o nível do personagem | [F2 class-features](.specs/features/class-features/spec.md) |
| 3 | Sabe calcular e preencher perícias e salvaguardas | [F3 skills-saves](.specs/features/skills-saves/spec.md) |
| 4 | Entende cada bônus (raça/classe) e o **tipo** dele | [F4 bonus-breakdown](.specs/features/bonus-breakdown/spec.md) |
| 5 | Salva, lista, abre e edita fichas (arquivos JSON, sem banco) | [F5 character-sheets](.specs/features/character-sheets/spec.md) |

## Fases
| Fase | Entrega | Gate de saída | Status |
|---|---|---|---|
| 0 — Fundação | AGENTS.md, rules, scripts, harness, arquitetura, specs | `verify.sh` verde (SKIP permitido) | ✅ |
| 1 — Scaffold | .NET 10 SDK; `DndBuilder.slnx` + 4 projetos + 3 de teste; `ng new` Angular 22; ESLint/Prettier; CI | `verify.sh --strict` verde com 1 teste trivial por projeto | ✅ aguardando revisão |
| 2 — Motor de regras | `Domain`: atributos, modificador, proficiência, perícias, salvaguardas, `DerivedValue` | `Domain.Tests` cobre `docs/domain/calculations.md` §1–6 | ✅ aguardando revisão |
| 3 — Catálogo | schemas + 9 raças (+ sub-raças) + 12 classes (níveis 1–20, classe base) | `Ruleset.Tests` verde; revisão humana vs. LdJ | ✅ aguardando revisão |
| 4 — API | endpoints de `docs/architecture/api-contract.md` (incl. CRUD de fichas em arquivo, F5) + OpenAPI | `Api.Tests` verde | ✅ aguardando revisão |
| 5 — Wizard | "Minhas fichas" + passos Raça → Classe → Atributos → Perícias/Salvaguardas → Ficha, com salvamento automático | Vitest verde; critérios F1–F5 demonstráveis | ✅ aguardando revisão |
| 6 — Qualidade | Playwright E2E, axe, polimento de textos | E2E dos 5 critérios verde | ✅ aguardando revisão |
| 7 — Robustez e UX pós-QA | Specs F6–F9 a partir do [relatório de QA](docs/qa/2026-10-20-teste-e2e/README.md) | Critérios F6–F9 com teste; `verify.sh --strict` verde | ✅ aprovada (2026-10-21) |
| 8 — Incremento de atributo | [F10 ability-score-improvement](.specs/features/ability-score-improvement/spec.md); tarefas em `.specs/project/tasks-fase-8.md` | Critérios F10 com teste; `verify.sh --strict` verde | ✅ aguardando revisão |

Dependências: 1 → 2 → (3 ∥ 4 parcialmente) → 5 → 6. Fases 2 e 3 podem correr em paralelo
depois que o schema (`content/schemas/`) estiver fechado.

### Fase 7 — features
| Feature (spec) | Resolve |
|---|---|
| [F6 input-validation](.specs/features/input-validation/spec.md) | Entrada inválida → 400, nunca 500; mensagens com nomes |
| [F7 sheet-file-integrity](.specs/features/sheet-file-integrity/spec.md) | Arquivo editado/copiado à mão → 422 claro; duplicar ficha |
| [F8 wizard-feedback](.specs/features/wizard-feedback/spec.md) | Erros visíveis por passo, rede × escolha, diálogos acessíveis |
| [F9 beginner-guidance](.specs/features/beginner-guidance/spec.md) | Contador da compra de pontos; selo "ainda não aplicado" |

## Pós-MVP (backlog, cada item exige spec)
- Antecedentes completos (Cap. 4) — hoje só proficiências manuais extras.
- Arquétipos/subclasses detalhados (depende de ADR-0004).
- ~~Aumento de atributo por nível (ASI)~~ → Fase 8 (F10). Talentos: regra opcional, quase toda fora do SRD 5.1 (ADR-0004).
- Magias por classe/nível; equipamento e CA calculada.
- Compartilhar ficha por link, multiusuário/login (exige banco — novo ADR); exportar PDF no layout oficial.
