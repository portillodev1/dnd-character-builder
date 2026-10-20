#!/usr/bin/env bash
# Sensor: nenhum .md de guia/doc/spec passa de 100 linhas; AGENTS.md ≤ 100.
# Uso: scripts/check-docs.sh
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
cd "$ROOT"
FAIL=0
while IFS= read -r f; do
  n=$(wc -l < "$f")
  if [ "$n" -gt 100 ]; then echo "FAIL: $f tem $n linhas (teto 100) — quebre em .md menores" >&2; FAIL=1; fi
done < <(find . -name "*.md" -not -path "./frontend/node_modules/*" -not -path "./.git/*" -not -path "./.claude/*" \
           -not -path "*/bin/*" -not -path "*/obj/*")
[ $FAIL = 0 ] && echo "check-docs: OK"
exit $FAIL
