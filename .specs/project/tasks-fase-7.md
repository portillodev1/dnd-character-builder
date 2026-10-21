# Tasks — Fase 7: Robustez e UX pós-QA (efêmero)

Fonte: specs F6–F9 (`.specs/features/{input-validation,sheet-file-integrity,wizard-feedback,
beginner-guidance}/spec.md`) · relatório `docs/qa/2026-10-20-teste-e2e/`.
Gate de saída: critérios F6–F9 com teste + `verify.sh --strict` → 0. Apagar após aprovada.
Regra: 1 comportamento por tarefa (≤ ~5 arquivos); teste de regressão que falha sem o fix.

## Bloqueios (antes de codar)
- [x] T0 — Aprovação humana (2026-10-20) das mudanças de contrato: F6 AC3/AC5, F7 AC1/AC2/AC5, F9 AC1/AC3.
  Depois: atualizar `docs/architecture/api-contract.md` (91 linhas → quebrar em
  `api-contract.md` + `api-contract-sheets.md` para ficar ≤ 100).
- [x] T1 — ADR-0010: origem da API por configuração (`proxy.conf.json` / mesma origem) — F8 RNF.

## F6 — Entrada inválida (back)
- [ ] T2 — Erro de binding → 400 ProblemDetails (`StatusCodeSelector`) — F6 AC1
- [ ] T3 — `level` da query `int?` validado no caso de uso — F6 AC2
- [ ] T4 — `choices` nulo = não escolhido; itens nulos → 400 — F6 AC3
- [ ] T5 — Faixa 3–18 sem `abilityMethod` (LdJ p. 13) — F6 AC4
- [ ] T6 — `PUT` sem `version` → 400 — F6 AC5
- [ ] T7 — Mensagens de escolha com nomes de perícia/atributo — F6 AC6
- [ ] T8 — OpenAPI documenta 400 em todos os endpoints + `gen-api.sh` + snapshot — F6 AC7

## F7 — Arquivos de ficha (back → front)
- [ ] T9 — `ResultStatus.Unreadable` + `GET` de arquivo com problema → 422 — F7 AC1
- [ ] T10 — `PUT` → 422 sem alterar o arquivo; `DELETE` de arquivo quebrado → 204 — F7 AC2
- [ ] T11 — Nome do arquivo é a identidade: id divergente = arquivo com problema — F7 AC3, AC4
- [ ] T12 — Log `Warning` com caminho e motivo — F7 AC6
- [ ] T13 — `POST /character-sheets/{id}/duplicate` — F7 AC5 (back)
- [ ] T14 — Botão "Duplicar" em "Minhas fichas" + E2E — F7 AC5 (front)

## F8 — Feedback do wizard (front)
- [ ] T15 — Mapa campo → passo + resumo de erros no topo com links — F8 AC1
- [ ] T16 — Baixar nível corta Especializações excedentes e avisa — F8 AC2
- [ ] T17 — Atributos: só inteiros, mensagem local — F8 AC3
- [ ] T18 — Status: cores por estado, 400 × rede/5xx, "Tentar de novo" — F8 AC4, AC5
- [ ] T19 — Renomear para vazio restaura o nome e avisa — F8 AC6
- [ ] T20 — "Próximo" bloqueado em `invalid`/`conflict` — F8 AC7
- [ ] T21 — Abrir ficha: 404 × 422 × rede — F8 AC8 (depende de T9)
- [ ] T22 — "Minhas fichas": erros de criar/apagar/duplicar em `role="alert"` — F8 AC9, AC10
- [ ] T23 — Descartar respostas antigas de `classFeatures` — F8 AC11
- [ ] T24 — `beforeunload` com salvamento pendente — F8 AC12
- [ ] T25 — Diálogos com `<dialog>` modal (foco, Tab, Esc) + E2E de teclado — F8 AC13
- [ ] T26 — Origem da API por config; remover reescrita de rota do E2E — F8 RNF (depende de T1)
- [ ] T27 — E2E: baixar nível com Especializações; API fora; axe nos estados novos — F8

## F9 — Guias para o iniciante (back → front)
- [ ] T28 — Domain: `pointBuy { spent, budget }` na ficha (LdJ p. 13) — F9 AC1 (back)
- [ ] T29 — Contador de pontos / mensagem acima do orçamento no passo Atributos — F9 AC1, AC2
- [ ] T30 — Domain/API: `manual: true` por grant + `warnings` — F9 AC3, AC4 (back)
- [ ] T31 — Selo "Ainda não aplicado" nos passos Classe e Ficha — F9 AC3 (front)
- [ ] T32 — Rótulo de "Meus bônus › Escolhas" sem origem duplicada — F9 AC5
- [ ] T33 — E2E: contador "restam" e selo no Ladino nv. 4; axe — F9

## Ordem e paralelismo
1. T0, T1 (decisões) → 2. F6 (T2–T8) e F7 back (T9–T13) em paralelo, arquivos distintos,
   exceto `Problems.cs`/`Program.cs` (T2 antes de T9) → 3. `gen-api.sh` uma vez → 4. F8 e F9 front.
- T15–T25 tocam `wizard-shell.ts`/store: uma sessão por vez nesses arquivos.
- F9 back (T28, T30) pode correr junto com F8 front.

## Fechamento
Atualizar `STATE.md` (handoff), `ROADMAP.md` (Fase 7 ✅) e o relatório de QA (itens corrigidos).
