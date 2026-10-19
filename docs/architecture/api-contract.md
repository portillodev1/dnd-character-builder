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
  "extraProficiencies": []
}
```

## Saída (recorte)
```json
{
  "abilities": { "constituicao": {
      "score": { "total": 16, "parts": [
        { "value": 14, "label": "Valor base", "bonusType": "base-score" },
        { "value": 2, "label": "Anão", "bonusType": "ability-score-increase",
          "source": { "book": "LdJ", "page": 20 } } ] },
      "modifier": 3 } },
  "proficiencyBonus": { "total": 2, "parts": [ { "value": 2, "label": "Nível 3",
      "bonusType": "proficiency-bonus", "source": { "book": "LdJ", "page": 15 } } ] },
  "savingThrows": { "destreza": { "proficient": true, "total": 4, "parts": [] } },
  "skills": { "furtividade": { "ability": "destreza", "proficiency": "expertise",
      "total": 6, "parts": [] } },
  "passivePerception": { "total": 15, "parts": [] },
  "features": [ { "id": "ataque-furtivo", "level": 1, "origin": "class" } ],
  "warnings": [ "Escolha 4 perícias da lista do Ladino (3 de 4)" ]
}
```

## Validação (400 com `errors` por campo)
- `level` 1–20; `baseAbilities` 1–20 antes de bônus raciais (método escolhido valida faixa).
- `raceId`/`classId`/`subraceId` existentes e coerentes (sub-raça pertence à raça).
- `skillChoices` ⊆ lista da classe (+ antecedente/raça); quantidade = permitida pela classe.
- `expertise` só se a classe/nível concede, e só em perícias já proficientes.
- Escolhas incompletas **não** são erro: viram `warnings` (o wizard é progressivo).
