# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff` primeiro (ou rode `scripts/bootstrap.sh`).

## Decisões (AD-NNN)
| ID | Data | Decisão | Ref |
|---|---|---|---|
| AD-001 | 2026-10-19 | Stack: Angular 22 + Node 24 LTS; .NET 10 LTS (ASP.NET Core Minimal APIs) | ADR-0001 |
| AD-002 | 2026-10-19 | API stateless sem banco; cálculo só no `Domain` | ADR-0002 |
| AD-003 | 2026-10-19 | Catálogo de regras em `content/*.json` com JSON Schema | ADR-0003 |
| AD-004 | 2026-10-19 | Conteúdo SRD 5.1 + resumos próprios; PDF não versionado (proposto) | ADR-0004 |
| AD-005 | 2026-10-19 | Skill de spec: `spec-driven-development` (`tlc-spec-driven` não instalada) | — |
| AD-006 | 2026-10-19 | Commit só com aprovação explícita do usuário | AGENTS.md |
| AD-007 | 2026-10-19 | UI usa termos da ficha oficial; sinônimos do livro via glossário | docs/domain/glossary.md |
| AD-008 | 2026-10-19 | Loop autônomo **não** acionado na Fase 0 (sem código; teste de necessidade falhou) | agentic-delivery |

## Eval gates disponíveis (de `HARNESS.md` §4)
`check-docs.sh` ✅ · `build.sh` / `lint.sh` / `test.sh` ⏳ (SKIP até a Fase 1) · `verify.sh --strict` ⏳

## Specs
| Feature | Fase spec-driven | Artefatos |
|---|---|---|
| projeto | Specify ✅ · Plan ✅ · Tasks ☐ | `.specs/project/spec.md`, `plan.md` |
| F1 race-traits | Specify ✅ | `.specs/features/race-traits/spec.md` |
| F2 class-features | Specify ✅ | `.specs/features/class-features/spec.md` |
| F3 skills-saves | Specify ✅ | `.specs/features/skills-saves/spec.md` |
| F4 bonus-breakdown | Specify ✅ | `.specs/features/bonus-breakdown/spec.md` |

## Handoff
- **Última unidade**: Fase 0 — Fundação (bootstrap, harness, arquitetura, specs). Nada commitado.
- **Bloqueios**: .NET SDK 10 não instalado na máquina; ADR-0004 item 4 aguarda decisão;
  perguntas abertas em `.specs/project/spec.md`.
- **Próxima unidade**: revisão humana das specs → Tasks da Fase 1 (Scaffold).
- **Gates**: `verify.sh` verde (com SKIP). `--strict` falha até o scaffold existir (esperado).
- **Budget/loop**: n/a.
