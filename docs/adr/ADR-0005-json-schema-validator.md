# ADR-0005: JsonSchema.Net para validar o catálogo `content/`

- **Status**: aceito
- **Data**: 2026-10-20
- **Decisores**: Gabriel Portillo

## Contexto
O ADR-0003 exige que `content/**/*.json` seja validado por JSON Schema no startup e em
`Ruleset.Tests`. O .NET 10 não traz validador de JSON Schema; precisamos de uma dependência.

## Decisão
Vamos usar **JsonSchema.Net 9.4.1** (json-everything) na `Infrastructure`, com schemas em
draft 2020-12. Ela é construída sobre `System.Text.Json` (sem Newtonsoft) e implementa o draft
completo, incluindo `$ref` entre arquivos.

**Licença**: código-fonte MIT, mas os binários do NuGet (v8+) seguem o *Open Source
Maintenance Fee Agreement* (`OSMFEULA.txt` no pacote): quem usa a biblioteca em atividade que
gera receita, com faturamento anual ≥ US$ 10 mil, paga uma taxa mensal. Este projeto
(pessoal, sem receita) está **isento**.

## Alternativas consideradas
- NJsonSchema — MIT puro, mas depende de Newtonsoft.Json e trabalha com draft-07.
- Corvus.JsonSchema — Apache-2.0 e draft 2020-12, mas gera código a partir do schema;
  pesado demais para um catálogo pequeno.

## Consequências
- **Positivas**: schemas legíveis em draft 2020-12; validação idêntica no teste e no startup.
- **Negativas / trade-offs**: risco de taxa se o projeto gerar receita acima do limite.
- **Follow-ups**: se o projeto virar produto com receita, reavaliar (pagar a taxa ou migrar
  para NJsonSchema/Corvus) em novo ADR.
