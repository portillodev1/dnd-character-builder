#!/usr/bin/env bash
# Reconstrói o contexto no início de uma sessão (anti-amnésia). Uso: scripts/bootstrap.sh
set -uo pipefail
source "$(dirname "$0")/_lib.sh"
cd "$ROOT"
echo "== Handoff (STATE.md)"
awk '/^## Handoff/{f=1} f' STATE.md 2>/dev/null || echo "(sem STATE.md)"
echo; echo "== Git"
git log --oneline -5 2>/dev/null; git status --short 2>/dev/null
echo; echo "== Toolchain"
echo "node:   $(node -v 2>/dev/null || echo ausente)"
echo "dotnet: $(dotnet --version 2>/dev/null || echo 'SDK ausente — instale .NET 10')"
echo; echo "== Scaffold"
has_back  && echo "backend:  OK" || echo "backend:  não scaffoldado"
has_front && echo "frontend: OK" || echo "frontend: não scaffoldado"
echo; echo "Próximo: leia AGENTS.md → Quick reference da tarefa."
