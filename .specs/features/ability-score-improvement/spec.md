# Spec F10: Incremento no Valor de Habilidade (ASI) por nível

Pós-MVP (Fase 8). Backlog do `ROADMAP.md` ("Aumento de atributo por nível"). Hoje a característica
sai com `manual: true` e o selo "anote à mão" (F9 AC3). Fórmula: `docs/domain/calculations.md` §1.
**Muda o contrato público** (AC2, AC5): exige aprovação humana antes de implementar.

## Objective
O iniciante escolhe na própria ficha onde aplicar cada Incremento no Valor de Habilidade da classe
(+2 em um atributo ou +1 em dois, sem passar de 20) e vê o aumento na conta de cada atributo.

## User stories
- Como jogador de nível 4+, quero aplicar meus Incrementos sem anotar nada à mão.
- Como iniciante, quero ver na conta da Destreza que +1 veio do Incremento do nível 4.

## Acceptance criteria
1. **Dado** classe e nível com Incrementos (ex.: Guerreiro nv. 6 → níveis 4 e 6, LdJ p. 85;
   Ladino nv. 10 → 4, 8 e 10, p. 91), **então** o passo Atributos mostra uma seção por
   Incremento, com dois "+1" a escolher entre os seis atributos (repetir = +2 no mesmo).
2. O rascunho ganha `abilityImprovements: { "<featureId>": ["destreza", "destreza"] }`. Cada
   escolha vira uma parcela `+1` do tipo `ability-score-increase`, origem `choice`, rótulo
   "Incremento no Valor de Habilidade (nível 4)" e a página da classe, aplicada **depois** dos
   aumentos raciais e em ordem de nível.
3. Teto 20 (LdJ p. 175): a parcela que passaria de 20 sai com valor 0 e rótulo "(limitado ao teto
   de 20)", como já acontece com bônus raciais; e a ficha avisa "Destreza já está em 20: o +1 do
   Incremento do nível 8 não soma nada."
4. Incremento com as 2 escolhas feitas não gera aviso; com 0 ou 1, aviso "Incremento no Valor de
   Habilidade (nível 4): escolha 2 aumentos (1 de 2)." O Incremento deixa de ser `manual`
   (o selo "anote à mão" some); a escolha de arquétipo continua `manual`.
5. `features[]` (de `/classes/{id}/features` e da ficha) ganha `abilityImprovement: bool` — o
   front sabe quais características abrem a seção do AC1 sem conhecer regra.
6. `400` em `abilityImprovements.<featureId>`: característica que não é Incremento da classe até
   o nível; mais de 2 escolhas; atributo inexistente ou vazio.
7. Trocar de classe limpa `abilityImprovements`; baixar o nível remove as escolhas de Incrementos
   acima do novo nível, na mesma mudança, com aviso de quais saíram (como F8 AC2).
8. "Meus bônus › Escolhas" lista cada +1: "+1 Destreza · Aumento de atributo · Incremento no
   Valor de Habilidade (nível 4) (Guerreiro)".

## Decisões
- Escolha como lista de 2 ids (não mapa atributo → pontos): repetir o id expressa o +2.
- A ordem de aplicação (raça → escolhas raciais → Incrementos por nível) define onde o teto corta.
- Sem mudar `content/`: os Incrementos já são os grants `ability-score-improvement` do catálogo.

## Testes exigidos
- `Domain.Tests` (Theory, LdJ p. 175): +2 num atributo, +1 em dois, teto em 20 (19 + 2 → 20 com
  parcela de 1 e de 0), ordem raça → Incremento, Guerreiro nv. 6 com dois Incrementos.
- `Domain.Tests`: validações do AC6; `manual` falso para Incremento; avisos do AC3/AC4.
- `Api.Tests`: `abilityImprovements` no `/compute` e no `PUT`; `abilityImprovement` nos features;
  `400` do AC6; `OpenApiSnapshotTests` + `scripts/gen-api.sh`.
- Vitest: seção do AC1 a partir de `abilityImprovement`; AC7 (classe e nível); rótulo do AC8.
- E2E: Guerreiro nv. 4 escolhe +2 Força e vê a conta; baixar para nv. 3 remove a escolha; axe.

## Out of scope
Talentos (regra opcional; quase todos fora do SRD 5.1 — ADR-0004, exige decisão do autor);
multiclasse; Incrementos de outras fontes (itens mágicos, dádivas).
