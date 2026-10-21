# Tasks — Fase 9: Fechamento do MVP e próximos incrementos (efêmero)

Fonte: `STATE.md` (próxima unidade) · `ROADMAP.md` (Pós-MVP) · ADR-0004 (SRD 5.1).
Gate de saída: `verify.sh --strict` → 0 em cada entrega. Apagar após aprovada.
Regra: 1 comportamento por tarefa (≤ ~5 arquivos). Cada item de backlog começa por spec
(skill `spec-driven-development`) e só depois vira código. Push só com aprovação explícita.

## A — Fechar o MVP (humano + agente)
- [x] T1 — Revisão do catálogo `content/` (9 raças, 12 classes) vs. Livro do Jogador — feita,
  sem divergências (`docs/qa/2026-10-21-revisao-catalogo/`); resta o humano amostrar páginas:
  amostrar páginas citadas em `source`; registrar divergências em `docs/qa/`.
- [ ] T2 — Revisão humana dos critérios F1–F5 no app (`.claude/launch.json`: `api` 5080, `web` 4200).
- [ ] T3 — Corrigir divergências de T2 (T1 não achou nenhuma) (1 correção = 1 commit, com teste).
- [ ] T4 — Aprovar Fases 1–6 e apagar `tasks-fase-1.md` … `tasks-fase-5.md`; ROADMAP ✅.
- [ ] T5 — Perguntar ao humano e então `git push` (antes, `git pull` se o remoto mudou).

## B — Deslocamento de classe (lacuna conhecida; pequena)
- [ ] T6 — Spec `class-speed-bonus`: Bárbaro (Movimento Rápido, nv. 5) e Monge (Movimento sem
  Armadura, nv. 2+), com página do LdJ e regra de "sem armadura" em `calculations.md`.
- [ ] T7 — Domain: parcela de deslocamento por nível com breakdown e tipo de bônus.
- [ ] T8 — Catálogo: dados `speedBonus` nas classes + schema + `Ruleset.Tests`.
- [ ] T9 — Front: deslocamento da ficha mostra a nova parcela; E2E + axe.

## C — Arquétipos/subclasses (maior; depende de decisão)
- [ ] T10 — Decisão: ADR sobre o escopo de arquétipos dentro do SRD 5.1 (um por classe?
  quais?). **Pergunte ao humano** antes de seguir (AGENTS.md → PERGUNTE QUANDO).
- [ ] T11 — Spec `class-archetypes` (escolha no nível certo, características por nível,
  fim do selo "anote à mão"); definir contrato `draft.archetype` e aprovar mudança de API.
- [ ] T12 — Schema + dados dos arquétipos aprovados em T10 (`content/`, `source` com página).
- [ ] T13 — Domain/API: validar arquétipo vs. classe/nível; features entram no breakdown.
- [ ] T14 — Front: passo Classe com escolha de arquétipo; E2E + axe.

## D — Backlog seguinte (cada item exige spec; ordem sugerida)
- [ ] T15 — Spec Antecedentes completos (Cap. 4): proficiências, idiomas, característica.
- [ ] T16 — Spec Magias por classe/nível (SRD 5.1, ADR-0004).
- [ ] T17 — Spec Equipamento e CA calculada.
- [ ] T18 — Spec Exportar PDF no layout oficial.
- [ ] T19 — Spec Compartilhar por link / login (exige banco → novo ADR; só se o humano quiser).

## Ordem e paralelismo
A (T1→T5) vem primeiro. B e C são independentes entre si, mas ambos tocam `class-step.ts` e o
catálogo de classes: fazer em série (B → C). D só depois, uma spec por vez.

## Fechamento
Atualizar `STATE.md` (handoff) e `ROADMAP.md` ao fim de cada bloco; apagar este arquivo após aprovação.
