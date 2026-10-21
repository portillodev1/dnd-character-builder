# Contrato da API (v1) — fichas salvas

Parte de [api-contract.md](api-contract.md) (validação e formato de `draft`/`sheet` lá).
Decisões: ADR-0006; specs F5 (`character-sheets`) e F7 (`sheet-file-integrity`).
Mudanças de F6/F7/F9 aprovadas em 2026-10-20 (Fase 7).

## Arquivos e rotas
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
- `version` ausente no `PUT` → `400` em `errors.version` (F6); divergente → `409` ProblemDetails.
- `draft` com valor inválido → `400` (mesmas regras de Validação acima); incompleto → `warnings`.
- `id` inexistente → `404`. Arquivo com problema aparece na lista com `"error"`, sem derrubar o `GET`.

## Arquivo com problema (F7)
JSON inválido, sem `name`/`draft`, `schemaVersion` desconhecido ou `id` interno ≠ nome do arquivo:
- `GET`/`PUT /{id}` → `422` ProblemDetails `"Arquivo da ficha com problema"` (o `PUT` não grava).
- `DELETE /{id}` → `204` (o app consegue limpar o arquivo). O nome do arquivo é a identidade.

## Duplicar (F7)
`POST /{id}/duplicate` (sem corpo) → `201` + `Location`; mesmo `draft`, `name` "<nome> (cópia)"
(até 80 caracteres), `version` 1. `id` inexistente → `404`; arquivo com problema → `422`.

## Campos aditivos da ficha derivada (F9)
- `pointBuy: { spent, budget } | null` — só com `abilityMethod = point-buy` e os 6 valores.
- `features[].manual: bool` — `true` quando o app ainda não aplica a característica
  (grants `ability-score-improvement`, `subclass-choice`); também gera um `warning`.
