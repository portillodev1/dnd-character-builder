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
- [x] T6b — Spec F11 aprovada pelo humano (2026-10-21; condição de armadura só no rótulo).
- [x] T7 — Catálogo: grant `speed-bonus` (`meters`, `condition`, `steps`) no schema + dados do
  Bárbaro e do Monge + `Ruleset.Tests` (degraus = coluna da tabela) — F11 AC4
- [x] T8 — Domain: `SpeedRules.Compute(race, subrace, class, level)` com parcelas e rótulos;
  `SheetCalculator` passa classe e nível; `calculations.md` §8 — F11 AC1–AC3, AC5
- [x] T9 — API: `/compute` Monge nv. 6 → 13,5 m (sem mudar DTO) — F11 AC4
- [x] T10 — Front: breakdown e "Meus bônus" com a parcela; Vitest + E2E (Monge nv. 6) + axe — F11 AC6–AC7

## C — Arquétipos/subclasses ([ADR-0011](../../docs/adr/ADR-0011-archetype-scope.md): todos os 41, em fatias)
- [x] T11 — Decisão do escopo: ADR-0011 aceito (2026-10-21), todos os 41 do LdJ.
- [x] T12 — Spec F12 `class-archetypes` escrita, com o contrato `draft.archetype` (ver "Contrato proposto").
- [x] T12b — Spec F12 e contrato `draft.archetype` aprovados pelo humano (2026-10-21).
- [x] T13 — Schema `archetype`, grants `archetype-slot`, loader, Domain (escolha, features, PV, avisos) e API
  (DTOs, `?archetype=`, `errors.archetype`, OpenAPI + `gen-api.sh`), com testes. Dados mínimos para provar
  o pipeline: Campeão e Linhagem Dracônica (já contam na fatia SRD da T15).
- [x] T14 — Front: passo Classe com a seção do arquétipo (opções, prévia, escolhas dele, limpeza ao trocar
  classe/nível), ficha com origem do arquétipo e grupo "Arquétipo — <nome>" em "Meus bônus"; Vitest + E2E + axe.
  (`ArchetypeDto` ganhou `choices[]`, aditivo, para a UI montar o seletor do dragão ancestral.)
- [ ] T15 — Dados, fatia 1: os 10 arquétipos do SRD que faltam (Campeão e Linhagem Dracônica já entraram na T13).
- [ ] T16 — Dados, fatias 2–4: os 29 restantes por classe (Clérigo/Mago em fatia própria: 7 e 8).
  Cada fatia só entra com `verify.sh --strict` verde e conferência contra o LdJ.

## D — Backlog seguinte (cada item exige spec; ordem sugerida)
- [ ] T17 — Spec Antecedentes completos (Cap. 4): proficiências, idiomas, característica.
- [ ] T18 — Spec Magias por classe/nível (SRD 5.1, ADR-0004).
- [ ] T19 — Spec Equipamento e CA calculada.
- [ ] T20 — Spec Exportar PDF no layout oficial.
- [ ] T21 — Spec Compartilhar por link / login (exige banco → novo ADR; só se o humano quiser).

## Ordem e paralelismo
A (T1→T5) vem primeiro. B e C são independentes entre si, mas ambos tocam `class-step.ts` e o
catálogo de classes: fazer em série (B → C). D só depois, uma spec por vez.

## Fechamento
Atualizar `STATE.md` (handoff) e `ROADMAP.md` ao fim de cada bloco; apagar este arquivo após aprovação.
