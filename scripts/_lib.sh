#!/usr/bin/env bash
# Helpers compartilhados pelos scripts. Não execute direto.
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BACK_SLN="$ROOT/backend/DndBuilder.slnx"
FRONT_DIR="$ROOT/frontend"
STRICT="${STRICT:-0}"   # STRICT=1 → parte não scaffoldada conta como falha

has_back()  { [ -f "$BACK_SLN" ]; }
has_front() { [ -f "$FRONT_DIR/package.json" ]; }

# skip <parte>: avisa que a parte ainda não existe; falha se STRICT=1.
skip() {
  echo "SKIP: $1 ainda não scaffoldado" >&2
  [ "$STRICT" = "1" ] && { echo "STRICT: tratando SKIP como falha" >&2; exit 1; }
  return 0
}
