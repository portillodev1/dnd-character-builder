#!/usr/bin/env bash
# Gate final: docs + build + lint + test + e2e. Saída 0 = pronto. Uso: scripts/verify.sh [--strict]
set -euo pipefail
cd "$(dirname "$0")"
[ "${1:-}" = "--strict" ] && export STRICT=1
./check-docs.sh
./build.sh
./lint.sh
./test.sh
./e2e.sh
echo "verify: OK"
