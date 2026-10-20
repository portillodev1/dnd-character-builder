# rules/implementation.md — carregada em: nova feature / fluxo / refactor

Regras testáveis para o agente ao implementar. Verificáveis por `scripts/verify.sh`.

## Regras
- Unidades pequenas: **1 comportamento por tarefa** (≤ ~5 arquivos). Sem "one-shot hero".
- Toda tarefa nasce de um critério de aceite em `.specs/features/<slug>/spec.md`.
  Sem critério → PARE e atualize a spec antes de codar.
- Siga os padrões já presentes no código. O linter é a fonte de estilo.
- **Cálculo de D&D só no back-end** (`DndBuilder.Domain`). O front exibe o que a API devolve.
- Todo valor derivado da ficha sai com **breakdown** (lista de parcelas com origem e tipo).
  Número sem explicação = critério 4 do projeto quebrado.
- Toda entrada externa é validada na borda (API); erro de negócio é explícito
  (`Result`/ProblemDetails), nunca `null` ou exceção genérica.
- Texto exibido ao usuário em **pt-BR**, termos iguais aos da ficha oficial
  (ex.: "Salvaguardas", "Bônus de Proficiência", "Sabedoria Passiva (Percepção)").
- Sem TODO/dead code no que entrega.
- Dependência nova ou decisão de arquitetura → PARE e registre ADR em `docs/adr/`.

## Fluxo de dados (não inverta)
```txt
content/*.json ──► Infrastructure (loader+schema) ──► Domain (regras puras)
                                                        │
Angular (wizard) ──HTTP──► Api (Minimal API) ──► Application (casos de uso)
```

## Exemplo — breakdown obrigatório
```txt
Perícia Furtividade = +5
  +3  Modificador de Destreza       (tipo: Modificador de habilidade)
  +2  Bônus de Proficiência nív. 1  (tipo: Proficiência — Ladino, p. 90)
```
