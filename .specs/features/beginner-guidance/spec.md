# Spec F9: Guias para o iniciante (pontos, pendências manuais)

Pós-MVP (Fase 7). Origem: `docs/qa/2026-10-20-teste-e2e/melhorias.md` (UX) e FE-14.
**Muda o contrato público** (AC1, AC3 — campos novos, aditivos): exige aprovação humana.

## Objective
O iniciante vê quanto do orçamento da compra de pontos já gastou e sabe quais características
do nível escolhido o app **ainda não aplica** — para não achar que os números estão errados.

## User stories
- Como iniciante, quero saber quantos pontos ainda posso gastar enquanto distribuo atributos.
- Como iniciante no nível 4+, quero saber que o Incremento no Valor de Habilidade não foi
  somado aos meus atributos e que preciso anotá-lo à mão.

## Acceptance criteria
1. **Dado** `abilityMethod = point-buy` com valores válidos, **quando** a ficha é calculada,
   **então** a resposta traz `pointBuy: { spent, budget }` (custo por valor e orçamento 27 —
   LdJ p. 13, `BaseScoreRules` no Domain) e o passo Atributos mostra
   "Pontos gastos: 23 de 27 (restam 4)". O front **não** soma custos (ADR-0002).
2. **Dado** compra de pontos acima do orçamento, **então** o passo mostra a mensagem da API
   ("Compra de pontos: 54 pontos gastos, o máximo é 27.") no lugar do contador.
3. **Dado** uma característica de classe com grant `ability-score-improvement` ou
   `subclass-choice` até o nível escolhido, **então** a API a marca com `manual: true` e o
   front exibe o selo "Ainda não aplicado nesta ficha — anote à mão" nos passos Classe e Ficha.
4. **Dado** ao menos uma característica `manual`, **então** a ficha traz um `warning` por item:
   "Incremento no Valor de Habilidade (nível 4) não está somado aos atributos: anote à mão." ou
   "Arquétipo de Ladino (nível 3): escolha à mão; o app ainda não aplica o arquétipo."
5. Em "Meus bônus › Escolhas" a origem aparece uma vez: "+1 Destreza · Aumento de atributo ·
   Aumento de Atributo (Meio-Elfo)" (sem "(Meio-Elfo) (Meio-Elfo)").

## Decisões
- `manual` é derivado dos `grants[]` já existentes no schema — **sem** mudar `content/`.
- `pointBuy` é `null` para outros métodos e enquanto faltarem valores.

## Testes exigidos
- `Domain.Tests` (Theory): `spent` para conjuntos com limites (todos 8 → 0; 15,15,15,8,8,8 → 27;
  um 14 → 7) — LdJ p. 13.
- `Domain.Tests`: `manual` para Ladino nv. 3 (Arquétipo) e nv. 4 (Incremento); nv. 1 sem `manual`.
- `Api.Tests`: `pointBuy` presente só em `point-buy`; `warnings` de AC4.
- Vitest: contador e mensagem de AC1/AC2; selo de AC3; rótulo de AC5.
- E2E: compra de pontos mostra "restam" e o selo aparece no Ladino nv. 4; axe.

## Out of scope
Aplicar ASI/talentos e escolher arquétipo (backlog "Aumento de atributo por nível" e
"Arquétipos/subclasses" no `ROADMAP.md`, cada um com spec própria).
