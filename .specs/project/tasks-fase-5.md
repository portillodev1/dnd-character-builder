# Tasks — Fase 5: Wizard (efêmero)

Fonte: specs F1–F5 · `docs/architecture/frontend.md` · `rules/frontend-angular.md` · ADR-0007.
Gate de saída: Vitest verde + critérios F1–F5 demonstráveis + `verify.sh --strict` → 0.
Apagar este arquivo após a Fase 5 aprovada.

- [x] T1 — ADR-0007: openapi-typescript; Api com `JsonNumberHandling.Strict`; `scripts/gen-api.sh`
  + `OpenApiSnapshotTests` (falha se o contrato mudar sem regenerar)
- [x] T2 — `core/api`: `models.ts` (apelidos dos tipos gerados), `RulesetService`, `SheetService`
- [x] T3 — `CharacterDraftStore`: debounce, "Salvo às", 400 por campo, 409 sem sobrescrever (F5 AC3–AC6)
- [x] T4 — "Minhas fichas": lista por data, nova, apagar com confirmação (F5 AC1, AC2, AC7)
- [x] T5 — Shell `/fichas/:id`: nome (F5 AC8), status, diálogo de conflito, passos
- [x] T6 — Raça: traços por origem, bônus tipados, escolhas que bloqueiam o avanço (F1)
- [x] T7 — Classe + nível: 12 classes, características até N, próximo nível, proficiências (F2)
- [x] T8 — Atributos: 3 métodos; total/modificador com breakdown vindo da API
- [x] T9 — Perícias/salvaguardas: contador, travas com motivo, Especialização, passiva, papel (F3)
- [x] T10 — Ficha: todo número com `bonus-breakdown`, "Meus bônus", legenda (F4)
- [x] T11 — Vitest: breakdown (tipo em texto, Enter/Espaço), store, lista/exclusão, raça, perícias, 409

## Verificado no navegador (2026-10-20)
Meio-Elfo (+1 For/Des, Atletismo/Percepção) → Ladino nv. 3 → conjunto padrão → perícias:
Força 11 = 10 + 1; Furtividade +7 (Especialização); PV 24; Sabedoria Passiva 15.
