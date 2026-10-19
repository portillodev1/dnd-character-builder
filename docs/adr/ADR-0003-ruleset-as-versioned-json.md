# ADR-0003: Catálogo de regras como JSON versionado com schema

- **Status**: aceito
- **Data**: 2026-10-19
- **Decisores**: Gabriel Portillo

## Contexto
9 raças, ~15 sub-raças e 12 classes com características em 20 níveis. Codificar isso
em C# mistura **dado** com **lógica** e dificulta revisão por quem conhece D&D mas não .NET.

## Decisão
Raças e classes ficam em `content/**/*.json`, validados por JSON Schema
(`content/schemas/`), embarcados como recurso no assembly da Infrastructure e carregados
no startup com falha rápida. Fórmulas genéricas ficam em C#; **dados** (bônus, traços,
features por nível) ficam no JSON. Bônus são tipados (`bonusType`), nunca texto livre.

## Alternativas consideradas
- Banco relacional com seed — exige DB (contraria ADR-0002).
- Classes C# por raça/classe — 21+ arquivos de código para o que é tabela.
- Consumir API pública de 5e (ex.: dnd5eapi) — dependência externa e conteúdo em inglês.

## Consequências
- **Positivas**: revisão de conteúdo por diff legível; gate `Ruleset.Tests` barra JSON inválido.
- **Negativas**: características com lógica especial (Especialização, Versatilidade) precisam
  de um vocabulário `grants[]` interpretado pelo Domain.
- **Follow-ups**: se `grants[]` crescer demais, avaliar DSL própria (novo ADR).
