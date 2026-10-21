# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff
- **Última unidade**: Fase 7 — **F7 sheet-file-integrity completa** (T9–T14): arquivo com problema
  (JSON inválido, sem `name`/`draft`, `schemaVersion` desconhecido, id ≠ nome do arquivo) → `422`
  no `GET`/`PUT`/duplicar sem gravar nada, `DELETE` limpa, motivo em pt-BR na lista e log `Warning`;
  `POST /{id}/duplicate` + botão "Duplicar". **Push pendente**. Próximo: F8 (T15).
- **F6 (anterior)**: entrada inválida → 400, nunca 500 (`6fa259a`, já no `origin/main`).
- **Specs da Fase 7**: F6–F9 + `.specs/project/tasks-fase-7.md` (`c1af3a6`).
- **QA do MVP (anterior)**: `docs/qa/2026-10-20-teste-e2e/`; FE-1 e BE-1 corrigidos (`34749c3`,
  já no `origin/main`).
- **Fase 6 (anterior)**: E2E + a11y (ADR-0009).
  - `frontend/e2e/`: 22 testes Playwright cobrindo F1–F5 + axe (WCAG 2.2 AA) em todos os passos,
    claro e escuro, e navegação por teclado. `scripts/e2e.sh` sobe API (5180, fichas em
    `frontend/e2e/.data/`) e front (4300) isolados; entra no `verify.sh` e no CI.
  - Polimento: "Minhas fichas" mostra nomes de raça/classe; passo Raça usa o deslocamento da
    sub-raça ("10,5 m"); mensagens de erro de servidor em linguagem de iniciante.
- **Lacunas F5 (anterior)**: deslocamento com breakdown, bônus de escolha com origem própria e
  limite de Especialização (`20ab4cb`).
- **Design do front** (polimento da Fase 6, adiantado): estética de manuscrito — velino com grão,
  rubricas em vermelho, azul-lápis, filetes duplos, capitular, passos em numerais romanos; tema
  escuro "à luz de vela". Fontes EB Garamond + IM Fell English via @fontsource (ADR-0008). Ícones
  de tipo de bônus agora são sinais tipográficos (sem emoji). Já no `origin/main`.
- **Fase 5 (anterior)**: wizard Angular + "Minhas fichas" (`.specs/project/tasks-fase-5.md`); tipos do
  front gerados do OpenAPI (ADR-0007); salvamento automático; `bonus-breakdown` em todo número.
  Testes: 20 front, 248 back.
- **Rodar**: `.claude/launch.json` (local, não versionado) tem `api` (5080) e `web` (4200).
- **Lacunas conhecidas**: aumentos de deslocamento de classe (Monge, Bárbaro) fora do MVP.
- **Revisão humana pendente**: Fases 1–6 (MVP completo).
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: revisão das specs F6–F9 → T0/T1 → Fase 7 na ordem do `tasks-fase-7.md`;
  depois o backlog pós-MVP do `ROADMAP.md` (cada item com spec).
- **Limpeza**: apagar `tasks-fase-1.md` … `tasks-fase-5.md` e `tasks-fase-7.md` quando aprovadas.
- **Gates**: `verify.sh --strict` verde.
