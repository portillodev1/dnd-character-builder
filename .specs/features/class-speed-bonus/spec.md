# Spec F11: Bônus de deslocamento da classe (Bárbaro e Monge)

Pós-MVP (Fase 9, bloco B). Lacuna conhecida do `STATE.md`: hoje o deslocamento da ficha só soma
raça e sub-raça; os aumentos de classe ficam só como texto. Fórmula: `docs/domain/calculations.md` §8.
**Não muda o contrato público**: o `speed` já devolve `parts[]` (api-contract.md); só ganha parcelas.

## Objective
Bárbaro e Monge veem o deslocamento da ficha já com o bônus da classe no nível atual, e a conta
mostra de onde ele veio e em que condição vale.

## User stories
- Como jogador de Monge nível 6, quero ver 13,5 m na ficha, não 9 m + conta de cabeça.
- Como iniciante, quero ver na conta do Deslocamento que o +3 m do Bárbaro vale "sem armadura pesada".

## Regras do livro (LdJ)
- **Bárbaro — Movimento Rápido** (p. 49): a partir do nível 5, +3 m "enquanto você não estiver
  vestindo uma armadura pesada".
- **Monge — Movimento sem Armadura** (p. 104): a partir do nível 2, +3 m "enquanto você não estiver
  usando armadura nem empunhando um escudo"; sobe na tabela do Monge: nv. 2 +3 · 6 +4,5 · 10 +6 ·
  14 +7,5 · 18 +9 m (o nv. 9 só acrescenta correr em paredes e líquidos, sem número).

## Acceptance criteria
1. **Dado** Humano (9 m) Bárbaro nv. 5, **então** Deslocamento = 12 m = 9 (Humano) + 3 (Movimento
   Rápido, p. 49); no nv. 4 continua 9 m e sem a parcela.
2. **Dado** Humano Monge nv. 6, **então** 13,5 m = 9 + 4,5 (Movimento sem Armadura, p. 104); os
   degraus são nv. 2, 6, 10, 14 e 18 (nv. 1 sem parcela; nv. 18 → 18 m).
3. A parcela soma ao total depois de raça e sub-raça (Anão Bárbaro nv. 5 → 7,5 + 3 = 10,5 m;
   Elfo da Floresta Monge nv. 2 → 10,5 + 3 = 13,5 m); invariante `total = Σ parts`.
4. Cada parcela tem `bonusType: speed`, origem Classe e rótulo com a condição: "Movimento Rápido
   (nível 5) — sem armadura pesada" / "Movimento sem Armadura (nível 6) — sem armadura nem escudo".
5. Outras classes e o nível 1 do Monge não mudam o deslocamento (nenhuma parcela extra).
6. "Meus bônus › Classe" e o breakdown do Deslocamento mostram a parcela com tipo "Deslocamento"
   e a página do LdJ (F4 AC2); o selo "anote à mão" não aparece (a característica não é `manual`).
7. Trocar de classe ou baixar o nível atualiza o total e a conta sem recarregar (a conta vem da API).

## Decisões
- **Condição assumida cumprida.** O app ainda não modela armadura/escudo (backlog "Equipamento e CA").
  O bônus entra sempre e a condição aparece no rótulo, para o jogador descontar se usar armadura.
  Quando houver equipamento, a condição passa a ser avaliada (nova spec).
- **Dado no catálogo, não no código** (ADR-0003): novo grant `speed-bonus` em `feature.grants`,
  com `meters` e `condition` (`no-heavy-armor` | `no-armor-no-shield`); o Monge usa `steps`
  (`[{level, meters}]`) em vez de ler a coluna de texto "+4,5 m" da tabela.
- `SpeedRules.Compute` passa a receber classe e nível; sem classe, o resultado é o de hoje.
- Sem mudança em DTO/OpenAPI: só novas parcelas em `speed.parts`. `gen-api.sh` não é necessário.

## Testes exigidos
- `Ruleset.Tests`: grant `speed-bonus` válido no schema; Bárbaro e Monge o têm; degraus do Monge
  conferem com a coluna `movimento-sem-armadura` da tabela da classe (mesmos 5 valores).
- `Domain.Tests` (Theory, LdJ p. 49 e 104): AC1–AC3, AC5 e a invariante `total = Σ parts`.
- `Api.Tests`: `/compute` Monge nv. 6 → `speed.total` 13,5 com a parcela do AC4.
- Vitest: breakdown do Deslocamento lista a parcela (rótulo, tipo, página).
- E2E: Monge nv. 6 mostra 13,5 m e a conta com a parcela; axe.

## Out of scope
Avaliar armadura/escudo de verdade; outros tipos de deslocamento (escalada, natação, voo);
deslocamento de arquétipos (ex.: Caminho dos Quatro Elementos); Pés Ligeiros e outros bônus raciais
(já tratados em §8).
