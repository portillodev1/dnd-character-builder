# Tasks — Fase 2: Motor de regras (efêmero)

Fonte: `.specs/project/plan.md` item 2 · `docs/domain/calculations.md` §1–6 ·
specs F3 (`skills-saves`) e F4 (`bonus-breakdown`, invariante).
Gate de saída: `Domain.Tests` cobre §1–6 e `bash scripts/verify.sh --strict` → 0.
Apagar este arquivo após a Fase 2 aprovada (política de limpeza do AGENTS.md).

- [x] T1 — Value objects: `AbilityScore` (1–30), `Modifier`, `Level` (1–20), `ProficiencyBonus`
  - Acceptance: construtores rejeitam valor inválido; mod = ⌊(v−10)/2⌋ (p. 175);
    proficiência = 2 + ⌊(nível−1)/4⌋ (p. 15).
  - Verify: testes de tabela nos limites (valor 1, 10, 11, 20, 30; níveis 1, 4, 5, 16, 17, 20).
  - Files: `Domain/AbilityScore.cs`, `Modifier.cs`, `Level.cs`, `ProficiencyBonus.cs`

- [x] T2 — Breakdown: `BonusType`, `SourceRef`, `BonusPart`, `DerivedValue`
  - Acceptance: `Total == ΣParts` por construção; sem parcelas → rejeita; enum de
    `docs/domain/bonus-types.md`.
  - Files: `Domain/BonusType.cs`, `SourceRef.cs`, `BonusPart.cs`, `DerivedValue.cs`

- [x] T3 — Atributos (§1–2): `Ability`, valor final com aumentos raciais e teto 20, modificador com breakdown
  - Acceptance: Anão da Colina Con 14 → 16 = 14 + 2; mod +3 (F4 AC7). Aumento que passa de 20
    é limitado e a parcela explica o teto (invariante preservada).
  - Files: `Domain/Ability.cs`, `AbilityScoreRules.cs`

- [x] T4 — Métodos de valor base (§1): conjunto padrão, compra de pontos (27, 8–15), manual (3–18)
  - Acceptance: validação devolve lista de erros em pt-BR; vazia = válido.
  - Files: `Domain/BaseScoreMethod.cs`, `BaseScoreRules.cs`

- [x] T5 — Salvaguardas (§4): `mod + (proficiente ? prof : 0)` com breakdown (p. 181)
  - Files: `Domain/SavingThrowRules.cs`

- [x] T6 — Perícias (§5): 18 perícias com atributo-chave; fator 0, ½ (arred. p/ baixo), 1, 2
  - Acceptance: Des 16, nível 1, Furtividade proficiente → +5 = +3 +2 (F3 AC4).
  - Files: `Domain/Skill.cs`, `SkillProficiency.cs`, `SkillRules.cs`

- [x] T7 — Sabedoria Passiva (Percepção) (§6): 10 + Percepção, com parcelas
  - Files: `Domain/PassivePerceptionRules.cs`

- [x] T8 — Propriedade `Total == ΣParts` para casos pseudoaleatórios (semente fixa)
  - Files: `Domain.Tests/DerivedValueInvariantTests.cs`
