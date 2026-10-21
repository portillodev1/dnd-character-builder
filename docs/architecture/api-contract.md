# Contrato da API (v1) — rascunho de arquitetura

Fonte da verdade quando houver código: `/openapi/v1.json`. Mudança de contrato
público → PERGUNTE (AGENTS.md). Erros sempre em **ProblemDetails** (RFC 9457).

## Endpoints
| Método | Rota | Critério | Retorna |
|---|---|---|---|
| GET | `/api/v1/races` | 1 | Lista resumida (`id`, `name`, `summary`, `subraces[]`) |
| GET | `/api/v1/races/{id}` | 1 | Raça com `traits[]`, `bonuses[]`, sub-raças detalhadas |
| GET | `/api/v1/classes` | 2 | Lista resumida (`id`, `name`, `hitDie`, `primaryAbilities`, `archetypeLevel`, `archetypes[]`) |
| GET | `/api/v1/classes/{id}/features?level=N[&archetype=<id>]` | 2 | Características com `level ≤ N`, proficiências, salvaguardas; com `archetype`, prévia com as dele |
| POST | `/api/v1/character-sheets/compute` | 3, 4 | Ficha derivada com breakdowns |
| GET | `/api/v1/character-sheets` | 5 | Fichas salvas (resumo), mais recente primeiro |
| POST | `/api/v1/character-sheets` | 5 | Cria ficha → `201` + `Location` |
| GET | `/api/v1/character-sheets/{id}` | 5 | Ficha salva + ficha derivada recalculada |
| PUT | `/api/v1/character-sheets/{id}` | 5 | Atualiza `name`/`draft` (exige `version`) |
| DELETE | `/api/v1/character-sheets/{id}` | 5 | Apaga o arquivo → `204` |
| POST | `/api/v1/character-sheets/{id}/duplicate` | F7 | Cópia da ficha → `201` + `Location` |
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
  "choices": { "proficiencia-com-ferramentas": ["Ferramentas de ferreiro"] },
  "abilityImprovements": { "incremento-no-valor-de-habilidade-4": ["destreza", "destreza"] }
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
  "speed": { "total": 7.5, "parts": [ { "value": 7.5, "label": "Anão", "bonusType": "speed",
      "source": { "book": "LdJ", "page": 20 } } ] },
  "features": [ { "id": "ataque-furtivo", "level": 1, "kind": "other", "scalingValue": "2d6" } ],
  "nextLevelFeatures": [], "traits": [ { "id": "visao-no-escuro", "origin": "race", "originName": "Anão" } ],
  "bonuses": [ { "target": "ability:constituicao", "value": 2, "bonusType": "ability-score-increase",
      "origin": "race", "originName": "Anão" } ],
  "warnings": [ "Escolha 4 perícias da lista do Ladino (3 de 4)" ]
}
```
`origin` de `bonuses[]`/`traits[]`: `race` · `subrace` · `class` · `choice` (escolha do jogador: +1 do
Meio-Elfo, perícias de raça/classe/antecedente e Especialização com `value` = Bônus de Proficiência,
e escolhas sem número como idioma/ferramenta em `traits[]`). `speed` é decimal (metros).
`GET /classes/{id}/features?level=N` traz `expertiseCount`: Especializações concedidas até N.

## Validação (400 com `errors` por campo) — spec F6
- Corpo malformado, tipo errado ou parâmetro ausente → `400` `problem+json`, **nunca `500`**.
- `level` 1–20 (também na query de `/features`); sem `abilityMethod`, `baseAbilities` 3–18
  (LdJ p. 13); com método, a faixa do método.
- `raceId`/`classId`/`subraceId` existentes e coerentes (sub-raça pertence à raça).
- `skillChoices` ⊆ lista da classe (+ antecedente/raça); quantidade = permitida pela classe.
- `expertise` só se a classe/nível concede, e só em perícias já proficientes.
- `choices.<id>: null` = não escolhido (warning); item `null` em listas → `400`.
- `abilityImprovements.<featureId>`: Incremento da classe até o nível, até 2 atributos (repetir =
  +2); fora disso → `400`. `features[].abilityImprovement` marca os Incrementos (spec F10).
- `archetype` (draft): id de arquétipo **da classe**, só com `level ≥ archetypeLevel`; senão `400` em
  `errors.archetype` (o mesmo na query `?archetype=` de `/features`). Escolhas dele vão em `choices`.
  Saída (F12): `sheet.archetype {id, name} | null`, `features[].archetype` (nome do dono ou `null`),
  `bonuses[].origin = "archetype"`; com arquétipo, os rótulos genéricos "Característica de …" somem.
- Mensagens citam perícias/atributos pelo nome ("Acrobacia"), nunca pelo id.
- Escolhas incompletas **não** são erro: viram `warnings` (o wizard é progressivo).

Fichas salvas (CRUD em arquivo, duplicar, `409`/`422`): [api-contract-sheets.md](api-contract-sheets.md).

