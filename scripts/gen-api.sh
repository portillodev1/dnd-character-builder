#!/usr/bin/env bash
# Regenera os tipos do front a partir do OpenAPI do back-end (ADR-0007).
# Uso: scripts/gen-api.sh — grava frontend/src/app/core/api/openapi.json e generated/schema.ts.
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
mkdir -p "$FRONT_DIR/src/app/core/api/generated"
UPDATE_OPENAPI=1 dotnet test --project "$ROOT/backend/tests/DndBuilder.Api.Tests" \
  --filter-method "*OpenApiDocument_MatchesFrontendSnapshot*" >/dev/null
(cd "$FRONT_DIR" && npm run --silent gen:api)
echo "gen-api: OK"
