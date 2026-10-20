#!/usr/bin/env bash
# Gate final: docs + build + lint + test. Saída 0 = pronto. Uso: scripts/verify.sh [--strict]
set -euo pipefail
cd "$(dirname "$0")"
[ "${1:-}" = "--strict" ] && export STRICT=1
./check-docs.sh
./build.sh
./lint.sh
./test.sh
echo "verify: OK"
