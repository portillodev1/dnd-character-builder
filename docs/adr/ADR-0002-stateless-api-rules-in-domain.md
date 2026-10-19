# ADR-0002: API stateless, sem banco; cálculo só no Domain

- **Status**: aceito
- **Data**: 2026-10-19
- **Decisores**: Gabriel Portillo

## Contexto
Os 4 critérios de pronto tratam de **entender** a ficha (traços, características por
nível, perícias, salvaguardas, bônus) — nenhum exige salvar fichas ou ter contas.
Fórmulas de D&D duplicadas em front e back divergem com o tempo.

## Decisão
A API é **stateless**: recebe o rascunho (`CharacterDraft`) e devolve a ficha derivada.
O rascunho vive no navegador (signals + `localStorage`). **Todo cálculo** fica no projeto
`DndBuilder.Domain` (puro, sem I/O); o front só renderiza o resultado e o breakdown.

## Alternativas consideradas
- Banco (PostgreSQL/SQLite) + contas — custo de auth/LGPD sem critério que justifique.
- Cálculo no front (sem back-end de regras) — contraria o requisito de back-end .NET e
  espalha regra em TypeScript sem a mesma testabilidade por tabela.

## Consequências
- **Positivas**: deploy simples, sem migrações; regra testável num só lugar.
- **Negativas**: uma chamada HTTP por mudança no wizard (mitigar com debounce).
- **Follow-ups**: salvar/compartilhar ficha → novo ADR com persistência.
