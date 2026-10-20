#!/usr/bin/env bash
# Lint + format check. Uso: scripts/lint.sh [--fix]
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
FIX=0; [ "${1:-}" = "--fix" ] && FIX=1

if has_back; then
  if [ $FIX = 1 ]; then dotnet format "$BACK_SLN"
  else dotnet format "$BACK_SLN" --verify-no-changes; fi
else skip "backend"; fi

if has_front; then
  (cd "$FRONT_DIR" && npx ng lint $([ $FIX = 1 ] && echo --fix) \
    && npx prettier $([ $FIX = 1 ] && echo --write || echo --check) .)
else skip "frontend"; fi
