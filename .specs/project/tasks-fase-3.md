# Tasks — Fase 3: Catálogo (efêmero)

Fonte: `.specs/project/plan.md` itens 3 e 7 · `docs/architecture/ruleset-data.md` ·
specs F1 (`race-traits`) e F2 (`class-features`) · ADR-0003, ADR-0004, ADR-0005.
Gate de saída: `Ruleset.Tests` verde + revisão humana vs. LdJ + `verify.sh --strict` → 0.
Apagar este arquivo após a Fase 3 aprovada (política de limpeza do AGENTS.md).

## Fatia 1 — schema + Anão + Ladino (plan item 3)
- [x] T1 — ADR-0005 + pacote JsonSchema.Net na Infrastructure
- [x] T2 — Schemas `common`, `race`, `class` e `skills` (draft 2020-12)
  - Acceptance: `bonusType`/`kind`/`grants[]` são enums fechados (`docs/domain/bonus-types.md`);
    `source.page` obrigatório; `description` ≤ 400 caracteres (resumo próprio, ADR-0004).
- [x] T3 — `content/skills.json`, `races/anao.json` (Colina + Montanha), `classes/ladino.json`
- [x] T4 — Infrastructure: `content/**` como embedded resource + `ContentValidator`
- [x] T5 — `Ruleset.Tests`: todo JSON valida no schema; `id` único; `skill:x` existe;
  2 salvaguardas; ≥ 1 feature no nível 1; colunas escalonáveis com 20 linhas;
  `source.page` dentro do capítulo (Cap. 2: 17–44; Cap. 3: 45–122; Cap. 7: 175–182)
- [x] T6 — Páginas conferidas no PDF do LdJ (página impressa = página do PDF + 1)

## Páginas conferidas (2026-10-20)
| Item | Página |
|---|---|
| Anão (raça) · traços · Colina (Tenacidade Anã) · Montanha | 18 · 20 · 20 · 20 |
| Ladino (classe) · tabela/proficiências · features nv. 1–18 · Golpe de Sorte e arquétipos | 89 · 90 · 91 · 92 |
| Perícias: Atletismo 177; Acrobacia/Furtividade 178; Prestidigitação/Arcanismo/História 179; Int/Sab/Atuação/Enganação 180; Intimidação/Persuasão 181 | 177–181 |
| Capítulos: 2 = 17–44 · 3 = 45–122 · 7 = 175–182 | — |
| Domain corrigido: valor base 13 · teto 20 175 · perícia 176 · passiva 177 | — |

## Fatia 2 — restante do catálogo (plan item 7, paraleliza por arquivo)
- [ ] T7 — 8 raças: elfo, halfling, humano, draconato, gnomo, meio-elfo, meio-orc, tiefling
- [ ] T8 — 11 classes: bárbaro, bardo, bruxo, clérigo, druida, feiticeiro, guerreiro, mago,
  monge, paladino, patrulheiro
- [ ] T9 — `Ruleset.Tests`: exatamente 9 raças e 12 classes
