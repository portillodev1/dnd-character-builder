# content/ — catálogo de regras (dados)

Raças e classes do D&D 5e como JSON validado por schema (ADR-0003). Preenchido nas
Fases 3 do `ROADMAP.md`. Licença/fonte do texto: ADR-0004 (resumos próprios, nunca transcrição).

| Pasta | Conteúdo |
|---|---|
| `schemas/` | `race.schema.json`, `class.schema.json`, `archetype.schema.json`, `common.schema.json` |
| `races/` | 1 arquivo por raça (`anao.json`, ...), sub-raças embutidas |
| `classes/` | 1 arquivo por classe (`ladino.json`, ...), features nv. 1–20 |
| `archetypes/` | 1 arquivo por arquétipo (`campeao.json`, ...), com `classId` (ADR-0011) |
| `skills.json` | 18 perícias → atributo-chave + sinônimos do livro |

Modelo: [docs/architecture/ruleset-data.md](../docs/architecture/ruleset-data.md) ·
Regras para a IA: [rules/ruleset-content.md](../rules/ruleset-content.md).
