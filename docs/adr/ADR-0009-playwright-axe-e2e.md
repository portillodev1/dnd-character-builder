# ADR-0009: E2E com Playwright e acessibilidade com axe-core

- **Status**: aceito
- **Data**: 2026-10-20
- **Decisores**: Gabriel Portillo

## Contexto
A Fase 6 (`ROADMAP.md`) exige E2E dos 5 critérios do MVP e verificação de acessibilidade
(WCAG 2.2 AA, `rules/frontend-angular.md`). Vitest cobre componentes isolados, mas não o
wizard ponta a ponta contra a API real nem regras de a11y que dependem do DOM renderizado.

## Decisão
Vamos usar como `devDependencies` exatas do `frontend/`:
- **@playwright/test 1.63.0** (Apache-2.0) — só Chromium;
- **@axe-core/playwright 4.13.0** (MPL-2.0) — regras `wcag2a/aa`, `wcag21a/aa`, `wcag22aa`.

Os testes ficam em `frontend/e2e/` e rodam por `scripts/e2e.sh`, chamado pelo `verify.sh`.
O Playwright sobe uma API **isolada** (porta 5180, fichas numa pasta temporária
`frontend/e2e/.data/`) e o front na porta 4300; o teste redireciona as chamadas de
`localhost:5080` para a 5180. Assim o E2E não toca nas fichas de desenvolvimento nem disputa
porta com os servidores de dev.

## Alternativas consideradas
- Cypress — runner próprio e mais pesado; Playwright já é o planejado em `HARNESS.md`.
- pa11y / Lighthouse CI — só a11y; não cobrem o fluxo do wizard.
- Mockar a API no E2E — mais rápido, mas não prova o fluxo real front + back.

## Consequências
- **Positivas**: os 5 critérios viram teste executável no gate; violação de a11y quebra o CI.
- **Negativas / trade-offs**: CI baixa o Chromium (~95 MB) e sobe o .NET; `verify.sh` fica
  ~1 min mais lento.
- **Follow-ups**: nenhum.
