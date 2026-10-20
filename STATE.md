# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff
- **Última unidade**: Fase 5 — wizard Angular + "Minhas fichas" (`.specs/project/tasks-fase-5.md`).
  Commitada localmente; **push pendente de aprovação**.
- **Feito**: tipos do front gerados do OpenAPI (ADR-0007, `scripts/gen-api.sh`; snapshot testado em
  `Api.Tests`); `CharacterDraftStore` com salvamento automático (debounce, 400 por campo, 409);
  rotas `/fichas` e `/fichas/:id/{raca,classe,atributos,pericias,ficha}`; `bonus-breakdown` em todo
  número; "Meus bônus" por origem; painel "Como preencher". Fluxo Meio-Elfo → Ladino nv. 3
  conferido no navegador contra a API real. 16 testes front, 230 back.
- **Rodar**: `.claude/launch.json` (local, não versionado) tem `api` (5080) e `web` (4200).
- **Lacunas conhecidas**: deslocamento sem breakdown (a API não o calcula); "Escolhas" em
  "Meus bônus" lista as escolhas do rascunho (a API marca bônus de escolha com origem da raça);
  Especialização: o front não sabe quantas a classe concede — a API avisa/recusa.
- **Revisão humana pendente**: Fases 1–5.
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: Fase 6 (Playwright E2E dos 5 critérios, axe, polimento de textos).
- **Limpeza**: apagar `tasks-fase-1.md` … `tasks-fase-5.md` quando aprovadas.
- **Gates**: `verify.sh --strict` verde.
