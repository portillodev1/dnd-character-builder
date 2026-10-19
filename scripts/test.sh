#!/usr/bin/env bash
# Roda os testes. Uso: scripts/test.sh [--back|--front] [--filter <padrão>] [--watch]
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
PART="all"; FILTER=""; WATCH=0
while [ $# -gt 0 ]; do case "$1" in
  --back) PART="back";; --front) PART="front";;
  --filter) FILTER="$2"; shift;;
  --watch) WATCH=1;;
  *) echo "arg desconhecido: $1" >&2; exit 2;;
esac; shift; done

if [ "$PART" != "front" ]; then
  if has_back; then
    if [ $WATCH = 1 ]; then dotnet watch test --project "$ROOT/backend/tests/DndBuilder.Domain.Tests"
    else dotnet test --solution "$BACK_SLN" ${FILTER:+--filter-method "*$FILTER*"}; fi
  else skip "backend"; fi
fi
if [ "$PART" != "back" ]; then
  if has_front; then
    (cd "$FRONT_DIR" && npx ng test $([ $WATCH = 1 ] && echo --watch || echo --watch=false) \
      ${FILTER:+--filter "$FILTER"})
  else skip "frontend"; fi
fi
