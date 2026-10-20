#!/usr/bin/env bash
# E2E do wizard (Playwright + axe, ADR-0009). Uso: scripts/e2e.sh [--grep <padrão>]
# Sobe API (5180) e front (4300) isolados; fichas numa pasta temporária.
set -euo pipefail
source "$(dirname "$0")/_lib.sh"
GREP=""
while [ $# -gt 0 ]; do case "$1" in
  --grep) GREP="$2"; shift;;
  *) echo "arg desconhecido: $1" >&2; exit 2;;
esac; shift; done

if has_back && has_front; then
  (cd "$FRONT_DIR" && npx playwright install chromium >/dev/null \
    && npx playwright test ${GREP:+--grep "$GREP"})
else skip "e2e"; fi
