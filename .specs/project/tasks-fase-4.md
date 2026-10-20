# Tasks — Fase 4: API (efêmero)

Fonte: `.specs/project/plan.md` itens 4–6 · `docs/architecture/api-contract.md` · specs F1–F5 ·
ADR-0002 (cálculo só no Domain), ADR-0006 (fichas em arquivo).
Gate de saída: `Api.Tests` verde + `verify.sh --strict` → 0. Catálogo usado: Anão + Ladino
(fatia 2 da Fase 3 corre depois, sem mudar código).
Apagar este arquivo após a Fase 4 aprovada.

- [x] T1 — Domain: modelo do catálogo (`Race`, `Subrace`, `CharacterClass`, `ClassFeature`, …),
  ids de atributo/perícia (`CatalogIds`) e filtro de features por nível (F2)
  - Verify: `Domain.Tests` com níveis 1, 2, 3, 20 do Ladino de teste.
- [x] T2 — Infrastructure: `JsonRulesetCatalog` (valida no schema + desserializa) e falha
  rápida no startup da Api
- [x] T3 — Api: `GET /races`, `GET /races/{id}`, `GET /classes`, `GET /classes/{id}/features?level`
  - Acceptance (F1/F2): detalhe do Anão tem Visão no Escuro; `orc-xyz` → 404; Ladino nv. 1 sem
    Ação Ardilosa, nv. 3 com ela e com o marco do arquétipo; `level=0` → 400.
- [x] T4 — Domain: `CharacterDraft` → `SheetCalculator` → `CharacterSheet` (atributos, proficiência,
  salvaguardas, perícias, passiva, PV §7, features, traços, warnings) e erros de validação
  - Acceptance: F3 AC1–7, F4 AC3/AC7; Anão da Colina Con 14 → 16; Furtividade +5.
- [x] T5 — Api: `POST /character-sheets/compute` (400 com `errors` por campo; incompleto → `warnings`)
  - Acceptance: perícia fora da lista → 400; Especialização sem proficiência → 400;
    contrato: nenhum número de ficha sem `parts` (F4).
- [x] T6 — Application + Infrastructure: `ICharacterSheetStore` / `FileCharacterSheetStore`
  (gravação atômica, `version`, arquivo corrompido não derruba a lista)
- [x] T7 — Api: CRUD `/character-sheets` (F5): 201 + Location, 404, 400, 409, 204
  - Verify: `Api.Tests` com pasta temporária.
- [x] T8 — Docs: contrato da API alinhado ao que foi implementado; STATE/ROADMAP

## Decisões de implementação (registradas aqui, sem ADR — seguem ADRs existentes)
- `CharacterDraft` usa ids em texto (`"destreza"`, `"lidar-com-animais"`); o Domain converte e
  devolve erro por campo para id desconhecido.
- Modificador sai como `DerivedValue` (clicável, F4 AC1), não número solto.
- Deslocamento fica no detalhe da raça (metros não inteiros); breakdown dele entra na Fase 5.
