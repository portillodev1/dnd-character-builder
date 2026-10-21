# Tasks — Fase 8: Incremento no Valor de Habilidade (efêmero)

Fonte: spec F10 (`.specs/features/ability-score-improvement/spec.md`) · `docs/domain/calculations.md`
§1 · `docs/architecture/api-contract.md`.
Gate de saída: critérios F10 com teste + `verify.sh --strict` → 0. Apagar após aprovada.
Regra: 1 comportamento por tarefa (≤ ~5 arquivos); teste que falha antes da implementação.

## Bloqueio (antes de codar)
- [ ] T0 — Aprovação humana das mudanças de contrato: `draft.abilityImprovements` (F10 AC2) e
  `features[].abilityImprovement` (F10 AC5). Depois: `api-contract.md` (validação) e
  `calculations.md` §1 (ASI deixa de ser "fase 2").

## Back-end (Domain → API)
- [ ] T1 — `CharacterDraft.AbilityImprovements` + validação em `ResolvedDraft` — F10 AC6
- [ ] T2 — Parcelas +1 por escolha, depois dos aumentos raciais, em ordem de nível — F10 AC2
- [ ] T3 — Teto 20: parcela 0 "(limitado ao teto de 20)" + aviso de +1 desperdiçado — F10 AC3
- [ ] T4 — Incremento deixa de ser `manual`; aviso "escolha 2 aumentos (k de 2)" — F10 AC4
- [ ] T5 — DTOs: `abilityImprovements` no rascunho e `abilityImprovement` nos features;
  `gen-api.sh` + snapshot — F10 AC2, AC5

## Front-end
- [ ] T6 — Seção "Incremento no Valor de Habilidade" no passo Atributos (dois +1 por
  Incremento, a partir de `abilityImprovement`) — F10 AC1
- [ ] T7 — Trocar de classe limpa; baixar o nível remove escolhas acima do nível, com aviso — F10 AC7
- [ ] T8 — Rótulo em "Meus bônus › Escolhas" — F10 AC8
- [ ] T9 — E2E: Guerreiro nv. 4 com +2 Força (conta e teto), baixar para nv. 3; axe — F10

## Ordem e paralelismo
T0 → T1 → T2 → T3 → T4 → T5 (mesmos arquivos do Domain, em série) → `gen-api.sh` → T6–T8
(front; T7 toca `class-step.ts`, os demais `abilities-step.ts`/`sheet-step.ts`) → T9.

## Fechamento
Atualizar `STATE.md` (handoff) e `ROADMAP.md` (Fase 8 ✅); apagar este arquivo após aprovação.
