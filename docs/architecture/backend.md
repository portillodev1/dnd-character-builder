# Back-end (.NET 10 LTS)

Clean Architecture enxuta, 4 projetos + 3 de teste. Regras para a IA: `rules/backend-dotnet.md`.

## Layout
```txt
backend/
  DndBuilder.slnx
  Directory.Build.props        # Nullable, TreatWarningsAsErrors, LangVersion
  Directory.Packages.props     # versões centralizadas (CPM)
  (../global.json)             # SDK 10 + runner Microsoft Testing Platform, na raiz do repo
  src/
    DndBuilder.Domain/         # motor de regras puro
    DndBuilder.Application/    # casos de uso + portas
    DndBuilder.Infrastructure/ # catálogo JSON (embedded) + validação de schema
    DndBuilder.Api/            # Minimal APIs, OpenAPI, ProblemDetails, CORS
  tests/
    DndBuilder.Domain.Tests/   # testes de tabela das fórmulas
    DndBuilder.Ruleset.Tests/  # content/*.json válidos + regras de integridade
    DndBuilder.Api.Tests/      # WebApplicationFactory, contrato HTTP + arquitetura
```

## Domain — conceitos centrais
| Tipo | Responsabilidade |
|---|---|
| `Ability` (enum 6) / `AbilityScore` / `Modifier` | Valor 1–30 e modificador `⌊(v−10)/2⌋` |
| `Level`, `ProficiencyBonus` | Nível 1–20 → +2…+6 |
| `Skill` (18) + `SkillAbilityMap` | Perícia → atributo-chave |
| `ProficiencyLevel` | Nenhuma · Metade (Versatilidade) · Proficiente · Especialização |
| `BonusPart` | `{ Value, Label, BonusType, SourceRef }` |
| `DerivedValue` | `{ Total, Parts[] }`, invariante `Total = ΣParts` |
| `CharacterDraft` | Entrada: raça, sub-raça, classe, nível, atributos base, escolhas |
| `SheetCalculator` | `CharacterDraft` + catálogo → `CharacterSheet` derivada |

## Application — casos de uso
- `ListRaces`, `GetRace(id)` → traços + bônus da raça/sub-raça (critério 1).
- `ListClasses`, `GetClassFeatures(id, level)` → características até o nível (critério 2).
- `ComputeSheet(draft)` → atributos, perícias, salvaguardas, bônus com breakdown (critérios 3 e 4).
- Porta: `IRulesetCatalog` (implementada na Infrastructure).

## Infrastructure
- `content/**/*.json` entra como **EmbeddedResource** no build.
- No startup: carrega, valida contra `content/schemas/`, indexa por `id`; falha rápida se inválido.

## Api
- `MapGroup("/api/v1/...")` por recurso; DTOs próprios; erros RFC 9457.
- OpenAPI nativo em `/openapi/v1.json` → fonte da geração de tipos do front.
- CORS liberado só para a origem do front configurada.
- Health: `GET /health`.

## Testes de arquitetura
`Api.Tests` garante via reflexão que `Domain` não referencia nenhum outro assembly
do projeto nem ASP.NET (sensor computacional, ver `HARNESS.md`).
