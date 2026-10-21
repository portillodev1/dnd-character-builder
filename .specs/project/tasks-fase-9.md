# Tasks — Fase 9: Fechamento do MVP e próximos incrementos (efêmero)

Fonte: `STATE.md` (próxima unidade) · `ROADMAP.md` (Pós-MVP) · ADR-0004 (SRD 5.1).
Gate de saída: `verify.sh --strict` → 0 em cada entrega. Apagar após aprovada.
Regra: 1 comportamento por tarefa (≤ ~5 arquivos). Cada item de backlog começa por spec
(skill `spec-driven-development`) e só depois vira código. Push só com aprovação explícita.

## A — Fechar o MVP (humano + agente)
- [x] T1 — Revisão do catálogo `content/` (9 raças, 12 classes) vs. Livro do Jogador — feita,
  sem divergências (`docs/qa/2026-10-21-revisao-catalogo/`); resta o humano amostrar páginas:
  amostrar páginas citadas em `source`; registrar divergências em `docs/qa/`.
- [x] T2 — Verificação dos critérios F1–F5 no app — feita, todos atendidos
  (`docs/qa/2026-10-21-t2-criterios-app/`); resta o humano conferir à mão.
- [x] T3 — Os 2 achados de UX da T2 corrigidos: painéis "Ver a conta" (um por vez; Esc e
  clique fora fecham, `bonus-breakdown.ts`) e renomear salva enquanto digita (`wizard-shell.ts`);
  4 testes Vitest novos.
- [ ] T4 — Aprovar Fases 1–6 e apagar `tasks-fase-1.md` … `tasks-fase-5.md`; ROADMAP ✅.
- [ ] T5 — Perguntar ao humano e então `git push` (antes, `git pull` se o remoto mudou).

## B — Deslocamento de classe ([F11 class-speed-bonus](../features/class-speed-bonus/spec.md))
- [x] T6 — Spec F11 escrita (Bárbaro p. 49, Monge p. 104; condição assumida cumprida).
- [ ] T6b — Aprovação humana da spec F11 (decisão: condição de armadura só no rótulo).
- [ ] T7 — Catálogo: grant `speed-bonus` (`meters`, `condition`, `steps`) no schema + dados do
  Bárbaro e do Monge + `Ruleset.Tests` (degraus = coluna da tabela) — F11 AC4
- [ ] T8 — Domain: `SpeedRules.Compute(race, subrace, class, level)` com parcelas e rótulos;
  `SheetCalculator` passa classe e nível; `calculations.md` §8 — F11 AC1–AC3, AC5
- [ ] T9 — API: `/compute` Monge nv. 6 → 13,5 m (sem mudar DTO) — F11 AC4
- [ ] T10 — Front: breakdown e "Meus bônus" com a parcela; Vitest + E2E (Monge nv. 6) + axe — F11 AC6–AC7

## C — Arquétipos/subclasses (maior; depende de decisão)
- [ ] T11 — Decisão: ADR sobre o escopo de arquétipos dentro do SRD 5.1 (um por classe?
  quais?). **Pergunte ao humano** antes de seguir (AGENTS.md → PERGUNTE QUANDO).
- [ ] T12 — Spec `class-archetypes` (escolha no nível certo, características por nível,
  fim do selo "anote à mão"); definir contrato `draft.archetype` e aprovar mudança de API.
- [ ] T13 — Schema + dados dos arquétipos aprovados em T11 (`content/`, `source` com página).
- [ ] T14 — Domain/API: validar arquétipo vs. classe/nível; features entram no breakdown.
- [ ] T15 — Front: passo Classe com escolha de arquétipo; E2E + axe.

## D — Backlog seguinte (cada item exige spec; ordem sugerida)
- [ ] T16 — Spec Antecedentes completos (Cap. 4): proficiências, idiomas, característica.
- [ ] T17 — Spec Magias por classe/nível (SRD 5.1, ADR-0004).
- [ ] T18 — Spec Equipamento e CA calculada.
- [ ] T19 — Spec Exportar PDF no layout oficial.
- [ ] T20 — Spec Compartilhar por link / login (exige banco → novo ADR; só se o humano quiser).

## Ordem e paralelismo
A (T1→T5) vem primeiro. B e C são independentes entre si, mas ambos tocam `class-step.ts` e o
catálogo de classes: fazer em série (B → C). D só depois, uma spec por vez.

## Fechamento
Atualizar `STATE.md` (handoff) e `ROADMAP.md` ao fim de cada bloco; apagar este arquivo após aprovação.
