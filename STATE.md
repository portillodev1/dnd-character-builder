# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff
- **Última unidade**: Fase 3 fatia 2 — catálogo completo (T7–T10 de `.specs/project/tasks-fase-3.md`).
  Commitada localmente; **push pendente de aprovação**.
- **Feito**: 9 raças (+ sub-raças; fora do SRD com `srd: false`) e 12 classes (features nv. 1–20,
  tabelas e espaços de magia), páginas conferidas no PDF. Domain aplica escolhas tipadas da raça
  (`choice.bonusType` + `value`: Meio-Elfo +1/+1 e 2 perícias). 229 testes back.
- **Revisão humana pendente** (gate da Fase 3): conteúdo vs. LdJ; Patrulheiro do PDF é a versão
  revisada (ver notas em `tasks-fase-3.md`).
- **Rodar a API**: `dotnet run --project backend/src/DndBuilder.Api` → http://localhost:5080;
  fichas em `data/fichas/` (fora do git; `Storage:SheetsPath`).
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: Fase 5 (wizard Angular + "Minhas fichas") sobre o catálogo completo.
- **Limpeza**: apagar `tasks-fase-1.md` … `tasks-fase-4.md` quando aprovadas.
- **Gates**: `verify.sh --strict` verde.
