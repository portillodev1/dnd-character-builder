#!/usr/bin/env bash
# Build de back e front. Uso: scripts/build.sh
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
if has_back; then dotnet build "$BACK_SLN" -c Release; else skip "backend"; fi
if has_front; then (cd "$FRONT_DIR" && npx ng build); else skip "frontend"; fi
