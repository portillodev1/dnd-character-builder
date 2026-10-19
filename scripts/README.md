# scripts/ — comandos repetíveis executáveis

Todo comando que o agente repetiria vira script aqui. O agente **roda o script**,
não monta o comando: economiza tokens, padroniza flags e dá saída 0/1 para os gates.
No Windows rode via **Git Bash** (`bash scripts/verify.sh`).

| Script | Faz | Flags |
|---|---|---|
| `test.sh` | Testes do back (`dotnet test`) e front (`ng test`, Vitest) | `--back`, `--front`, `--filter <p>`, `--watch` |
| `lint.sh` | `dotnet format --verify-no-changes` + `ng lint` + Prettier | `--fix` |
| `build.sh` | `dotnet build -c Release` + `ng build` | — |
| `check-docs.sh` | Sensor: nenhum `.md` acima de 100 linhas | — |
| `verify.sh` | **Gate**: check-docs + build + lint + test | `--strict` |
| `bootstrap.sh` | Reconstrói contexto: handoff, git, toolchain, scaffold | — |
| `_lib.sh` | Helpers (detecção de back/front, `skip`) — não executar | — |

## Fase atual: só estrutura
Enquanto `backend/DndBuilder.slnx` e `frontend/package.json` não existem, os scripts
imprimem `SKIP: ... ainda não scaffoldado` e seguem (saída 0). Use `--strict`
(ou `STRICT=1`) para que SKIP conte como falha — **obrigatório a partir da Fase 1
do `ROADMAP.md`**.
