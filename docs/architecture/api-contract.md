# Contrato da API (v1) — rascunho de arquitetura

Fonte da verdade quando houver código: `/openapi/v1.json`. Mudança de contrato
público → PERGUNTE (AGENTS.md). Erros sempre em **ProblemDetails** (RFC 9457).

## Endpoints
| Método | Rota | Critério | Retorna |
|---|---|---|---|
| GET | `/api/v1/races` | 1 | Lista resumida (`id`, `name`, `summary`, `subraces[]`) |
| GET | `/api/v1/races/{id}` | 1 | Raça com `traits[]`, `bonuses[]`, sub-raças detalhadas |
| GET | `/api/v1/classes` | 2 | Lista resumida (`id`, `name`, `hitDie`, `primaryAbilities`) |
| GET | `/api/v1/classes/{id}/features?level=N` | 2 | Características com `level ≤ N`, proficiências, salvaguardas |
| POST | `/api/v1/character-sheets/compute` | 3, 4 | Ficha derivada com breakdowns |
| GET | `/api/v1/character-sheets` | 5 | Fichas salvas (resumo), mais recente primeiro |
| POST | `/api/v1/character-sheets` | 5 | Cria ficha → `201` + `Location` |
| GET | `/api/v1/character-sheets/{id}` | 5 | Ficha salva + ficha derivada recalculada |
| PUT | `/api/v1/character-sheets/{id}` | 5 | Atualiza `name`/`draft` (exige `version`) |
| DELETE | `/api/v1/character-sheets/{id}` | 5 | Apaga o arquivo → `204` |
| GET | `/health` | — | 200 OK |

## `POST /character-sheets/compute` — entrada
```json
{
  "raceId": "anao", "subraceId": "anao-da-colina",
  "classId": "ladino", "level": 3,
  "baseAbilities": { "forca": 10, "destreza": 15, "constituicao": 14,
                     "inteligencia": 12, "sabedoria": 13, "carisma": 8 },
  "skillChoices": ["furtividade", "percepcao", "acrobacia", "enganacao"],
  "expertise": ["furtividade", "percepcao"],
  "extraProficiencies": ["intuicao", "investigacao"],
  "abilityMethod": "standard-array",
  "choices": { "proficiencia-com-ferramentas": ["Ferramentas de ferreiro"] }
}
```

## Saída (recorte)
```json
{
  "abilities": { "constituicao": {
      "score": { "total": 16, "parts": [
        { "value": 14, "label": "Valor base", "bonusType": "base-score",
          "source": { "book": "LdJ", "page": 13 } },
        { "value": 2, "label": "Anão", "bonusType": "ability-score-increase",
          "source": { "book": "LdJ", "page": 20 } } ] },
      "modifier": { "total": 3, "parts": [ { "value": 3, "bonusType": "ability-modifier", "…": "…" } ] } } },
  "proficiencyBonus": { "total": 2, "parts": [ { "value": 2, "label": "Nível 3",
      "bonusType": "proficiency-bonus", "source": { "book": "LdJ", "page": 15 } } ] },
  "savingThrows": { "destreza": { "proficient": true, "total": 4, "parts": [] } },
  "skills": { "furtividade": { "sheetLabel": "Furtividade (Des)", "ability": "destreza",
      "proficiency": "expertise", "proficiencySources": ["Ladino"], "total": 6, "parts": [] } },
  "passivePerception": { "total": 15, "parts": [] }, "hitPoints": { "total": 30, "parts": [] },
  "features": [ { "id": "ataque-furtivo", "level": 1, "kind": "other", "scalingValue": "2d6" } ],
  "nextLevelFeatures": [], "traits": [ { "id": "visao-no-escuro", "origin": "race", "originName": "Anão" } ],
  "bonuses": [ { "target": "ability:constituicao", "value": 2, "bonusType": "ability-score-increase",
      "origin": "race", "originName": "Anão" } ],
  "warnings": [ "Escolha 4 perícias da lista do Ladino (3 de 4)" ]
}
```

## Validação (400 com `errors` por campo)
- `level` 1–20; `baseAbilities` 1–20 antes de bônus raciais (método escolhido valida faixa).
- `raceId`/`classId`/`subraceId` existentes e coerentes (sub-raça pertence à raça).
- `skillChoices` ⊆ lista da classe (+ antecedente/raça); quantidade = permitida pela classe.
- `expertise` só se a classe/nível concede, e só em perícias já proficientes.
- Escolhas incompletas **não** são erro: viram `warnings` (o wizard é progressivo).

## Fichas salvas (ADR-0006, spec F5)
Cada ficha é um arquivo `<SheetsPath>/<id>.json` com **só as escolhas**; a ficha derivada é
recalculada a cada `GET`. `{id}` é GUID (`{id:guid}` na rota — não colide com `/compute`).
```json
// POST /api/v1/character-sheets  (corpo; draft pode estar incompleto)
{ "name": "Thorin", "draft": { "raceId": "anao", "classId": "ladino", "level": 1 } }
// GET /api/v1/character-sheets
[ { "id": "0199f6c2-…", "name": "Thorin", "raceId": "anao", "classId": "ladino",
    "level": 1, "updatedAt": "2026-10-20T18:42:10Z" } ]
// GET /api/v1/character-sheets/{id}
{ "id": "0199f6c2-…", "name": "Thorin", "version": 3, "createdAt": "…", "updatedAt": "…",
  "draft": { "…": "mesmo formato da entrada do /compute" },
  "sheet": { "…": "mesmo formato da saída do /compute (com warnings)" } }
// PUT /api/v1/character-sheets/{id}
{ "name": "Thorin", "version": 3, "draft": { "…": "…" } }   // → 200 com version 4
```
- `version` divergente → `409` ProblemDetails (a outra aba salvou antes).
- `draft` com valor inválido → `400` (mesmas regras de Validação acima); incompleto → `warnings`.
- `id` inexistente → `404`. Arquivo corrompido aparece na lista com `"error"`, sem derrubar o `GET`.
