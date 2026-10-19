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

## Modo estrito (padrão desde a Fase 1)
Back-end e front-end existem, então use sempre `scripts/verify.sh --strict` (também é o que o CI
roda): se `backend/DndBuilder.slnx` ou `frontend/package.json` sumirem, o `SKIP` vira falha.

## Notas da stack
- Testes .NET usam **Microsoft Testing Platform** (`global.json` na raiz). `--filter <p>` vira
  `--filter-method "*<p>*"` no back e `--filter <p>` no Vitest.
- Front: `npm ci` em `frontend/` antes do primeiro build em máquina nova.
