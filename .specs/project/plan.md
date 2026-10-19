# Plan: D&D Character Builder

Fase **Plan** do `spec-driven-development`. A arquitetura em si está em
`docs/architecture/` (não duplicada aqui); este arquivo cobre **ordem, riscos e checkpoints**.

## Componentes e dependências
```txt
content/schemas ──► content/*.json ──► Infrastructure (loader)
                                             │
Domain (fórmulas, DerivedValue) ◄────────────┤
        │                                    │
Application (casos de uso) ──► Api (endpoints, OpenAPI) ──► frontend/core/api (gerado)
                                                               │
                                       state (draft store) ──► features/* ──► shared/ui
```

## Ordem de implementação (fatias verticais quando possível)
1. **Scaffold** (Fase 1): solução, projetos, Angular, lint, CI, 1 teste trivial cada.
2. **Domain núcleo**: `AbilityScore`, `Modifier`, `Level`, `ProficiencyBonus`, `DerivedValue`.
3. **Schema do catálogo** + 1 raça (Anão) + 1 classe (Ladino) — dados mínimos de ponta a ponta.
4. **Fatia F1** (raça): `GET /races/{id}` → passo Raça no wizard, com Anão.
5. **Fatia F2** (classe): `GET /classes/{id}/features?level` → passo Classe+Nível, com Ladino.
6. **Fatia F3+F4** (ficha): `POST /compute` → passos Atributos, Perícias/Salvaguardas, Ficha.
7. **Completar catálogo**: demais 8 raças e 11 classes (pode paralelizar por arquivo).
8. **E2E + a11y**.

## Paralelizável
- Item 7 (um arquivo JSON por agente/sessão; schema já fechado).
- Front (mock do OpenAPI) ∥ back a partir do item 3, se o contrato estiver congelado.

## Riscos e mitigação
| Risco | Mitigação |
|---|---|
| Regra transcrita errada do LdJ | `source.page` obrigatório + revisão humana do JSON + testes de tabela |
| Características com lógica especial (Especialização, Versatilidade, escolhas) | Vocabulário `grants[]`/`choice` fechado no schema; testes específicos |
| Licença do conteúdo | ADR-0004; resumos próprios; PDF fora do repo |
| Wizard confuso para iniciante | Textos "O que é isso?" + glossário; teste de usabilidade com 1–2 iniciantes |
| Duas toolchains no Windows | Scripts bash via Git Bash; CI em Linux |
| .NET 10 SDK ausente localmente | Pré-requisito da Fase 1 (`scripts/bootstrap.sh` avisa) |

## Checkpoints de verificação
- Após 1: `verify.sh --strict` verde.
- Após 2: tabela §2–3 de `calculations.md` coberta.
- Após 4/5/6: critérios de aceite de F1/F2/F3/F4 demonstrados no navegador + testes verdes.
- Após 7: `Ruleset.Tests` verde para 9 raças e 12 classes; revisão humana assinada.

## Próximo passo
Revisão humana desta spec/plan → gerar `tasks.md` da Fase 1 (efêmero).
