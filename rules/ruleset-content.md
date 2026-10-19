# rules/ruleset-content.md — carregada em: dados de raça/classe em `content/`

O catálogo de regras é **dado, não código** (ADR-0003). Licenciamento: ADR-0004.
Modelo detalhado (humanos): `docs/architecture/ruleset-data.md`.

## Regras
- Um arquivo por entidade: `content/races/<id>.json`, `content/classes/<id>.json`.
  `id` em kebab-case sem acento (`meio-elfo`, `patrulheiro`).
- Todo arquivo valida contra `content/schemas/*.schema.json` (gate: `Ruleset.Tests`).
- Toda característica/bônus tem `source`: `{ "book": "LdJ", "page": <n> }`.
- Bônus são **tipados** (enum `bonusType`, ver `docs/domain/bonus-types.md`) — nunca texto livre.
- Características de classe têm `level` (1–20). O filtro "até o nível N" é da API, não do JSON.
- `description` é **resumo próprio** (≤ 3 frases, linguagem para iniciante) — nunca transcrição
  do livro. Texto integral fora do SRD 5.1 → PERGUNTE antes (ADR-0004).
- Mudou o schema? Atualize schema + teste + doc no mesmo commit.

## Exemplo — trecho de raça
```json
{
  "id": "anao", "name": "Anão", "source": { "book": "LdJ", "page": 18 },
  "bonuses": [
    { "target": "ability:constituicao", "value": 2,
      "bonusType": "ability-score-increase", "source": { "book": "LdJ", "page": 20 } }
  ],
  "traits": [
    { "id": "visao-no-escuro", "name": "Visão no Escuro", "kind": "sense",
      "description": "Enxerga na penumbra a até 18 m como se fosse luz plena.",
      "source": { "book": "LdJ", "page": 20 } }
  ],
  "subraces": ["anao-da-colina", "anao-da-montanha"]
}
```
