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
| AD-013 | 2026-10-20 | Sem banco; fichas salvas como JSON em `data/fichas/` (só escolhas, recalcula ao ler); código na Fase 4 | ADR-0006 |

## Eval gates disponíveis (de `HARNESS.md` §4)
`check-docs.sh` ✅ · `build.sh` ✅ · `lint.sh` ✅ · `test.sh --back` ✅ (224) · `test.sh --front` ✅ (1) · `verify.sh --strict` ✅ (exige Node 24) · CI `verify.yml` ⏳ (roda no próximo push)

## Specs
| Feature | Fase spec-driven | Artefatos |
|---|---|---|
| projeto | Specify ✅ · Plan ✅ · Tasks Fase 1 ✅ · Tasks Fase 2 ✅ · Tasks Fase 3 (fatia 1) ✅ · Tasks Fase 4 ✅ | `.specs/project/spec.md`, `plan.md` |
| F1 race-traits | Specify ✅ | `.specs/features/race-traits/spec.md` |
| F2 class-features | Specify ✅ | `.specs/features/class-features/spec.md` |
| F3 skills-saves | Specify ✅ | `.specs/features/skills-saves/spec.md` |
| F4 bonus-breakdown | Specify ✅ | `.specs/features/bonus-breakdown/spec.md` |
| F5 character-sheets | Specify ✅ | `.specs/features/character-sheets/spec.md` |

## Handoff
- **Última unidade**: Fase 4 — API (T1–T8 de `.specs/project/tasks-fase-4.md`). Commitada;
  **push pendente de aprovação**.
- **Feito**: Domain `Catalog/*` + `Sheets/` (`CharacterDraft` → `SheetCalculator` → `CharacterSheet`,
  PV §7, erros por campo e warnings); Application (`Result<T>`, 10 casos de uso, portas
  `IRulesetCatalog`/`ICharacterSheetStore`); Infrastructure (`JsonRulesetCatalog` com falha rápida,
  `FileCharacterSheetStore`); Api com 8 rotas + OpenAPI. 224 testes back.
- **Rodar a API**: `dotnet run --project backend/src/DndBuilder.Api` → http://localhost:5080;
  fichas em `data/fichas/` (fora do git; `Storage:SheetsPath`).
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: Fase 3 fatia 2 (8 raças + 11 classes, paralelizável por arquivo) e/ou
  Fase 5 (wizard Angular + "Minhas fichas") sobre Anão + Ladino.
- **Limpeza**: apagar `tasks-fase-1.md` … `tasks-fase-4.md` quando aprovadas.
- **Gates**: `verify.sh --strict` verde.
