# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff` primeiro (ou rode `scripts/bootstrap.sh`).

## Decisões (AD-NNN)
| ID | Data | Decisão | Ref |
|---|---|---|---|
| AD-001 | 2026-10-19 | Stack: Angular 22 + Node 24 LTS; .NET 10 LTS (ASP.NET Core Minimal APIs) | ADR-0001 |
| AD-002 | 2026-10-19 | API stateless sem banco; cálculo só no `Domain` | ADR-0002 |
| AD-003 | 2026-10-19 | Catálogo de regras em `content/*.json` com JSON Schema | ADR-0003 |
| AD-004 | 2026-10-19 | Conteúdo SRD 5.1 + resumos próprios; PDF não versionado (aceito) | ADR-0004 |
| AD-005 | 2026-10-19 | Skill de spec: `spec-driven-development` (`tlc-spec-driven` não instalada) | — |
| AD-006 | 2026-10-19 | ~~Commit só com aprovação~~ — substituído por AD-009 | AGENTS.md |
| AD-007 | 2026-10-19 | UI usa termos da ficha oficial; sinônimos do livro via glossário | docs/domain/glossary.md |
| AD-008 | 2026-10-19 | Loop autônomo **não** acionado na Fase 0 (sem código; teste de necessidade falhou) | agentic-delivery |
| AD-009 | 2026-10-19 | Commit livre com `verify.sh` verde; **push só com aprovação** | AGENTS.md |
| AD-010 | 2026-10-19 | Sub-raças/arquétipos fora do SRD: nome + mecânica resumida | ADR-0004 |
| AD-011 | 2026-10-19 | Atributos no MVP: conjunto padrão, compra de pontos (27) e manual | spec projeto |
| AD-012 | 2026-10-19 | Antecedente no MVP = marcar 2 perícias; deploy só local; PDF pós-MVP | spec projeto |

## Eval gates disponíveis (de `HARNESS.md` §4)
`check-docs.sh` ✅ · `build.sh` ✅ · `lint.sh` ✅ · `test.sh --back` ✅ (6) · `test.sh --front` ✅ (1) · `verify.sh --strict` ✅ · CI `verify.yml` ⏳ (roda no próximo push)

## Specs
| Feature | Fase spec-driven | Artefatos |
|---|---|---|
| projeto | Specify ✅ · Plan ✅ · Tasks Fase 1 ✅ | `.specs/project/spec.md`, `plan.md` |
| F1 race-traits | Specify ✅ | `.specs/features/race-traits/spec.md` |
| F2 class-features | Specify ✅ | `.specs/features/class-features/spec.md` |
| F3 skills-saves | Specify ✅ | `.specs/features/skills-saves/spec.md` |
| F4 bonus-breakdown | Specify ✅ | `.specs/features/bonus-breakdown/spec.md` |

## Handoff
- **Última unidade**: Fase 1 — Scaffold (T1–T7 de `.specs/project/tasks-fase-1.md`). Commitado; **push pendente de aprovação**.
- **Feito**: `backend/DndBuilder.slnx` (.NET 10, 4 src + 3 testes xUnit v3/MTP, `/health`, OpenAPI,
  ArchitectureTests); `frontend/` Angular 22.2 (zoneless, Vitest, angular-eslint + regras de fronteira,
  shell do wizard com 5 rotas lazy); CI `.github/workflows/verify.yml`.
- **Bloqueios**: nenhum. CI só será validado de verdade após o push.
- **Próxima unidade**: Fase 2 — motor de regras no `Domain` (`.specs/project/plan.md` item 2),
  guiado por `docs/domain/calculations.md` §1–6. Gerar `tasks-fase-2.md` antes de codar.
- **Limpeza**: apagar `.specs/project/tasks-fase-1.md` quando a Fase 1 for aprovada.
- **Gates**: `verify.sh --strict` verde.
