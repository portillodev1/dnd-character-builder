# ADR-0001: Monorepo com Angular 22 + .NET 10 LTS

- **Status**: aceito
- **Data**: 2026-10-19
- **Decisores**: Gabriel Portillo

## Contexto
O projeto exige front-end Angular e back-end .NET nas versões LTS mais recentes. Em
out/2026: **.NET 10** é a LTS vigente (suporte até nov/2028; .NET 11 é STS). Angular
**22** é a major atual (cada major recebe 18 meses de suporte, 12 deles em LTS).
Node **24** é a LTS ativa exigida pelo Angular CLI.

## Decisão
Um único repositório com `frontend/` (Angular 22, standalone, zoneless, signals, Vitest),
`backend/` (.NET 10, ASP.NET Core Minimal APIs, solução `.slnx`, xUnit v3) e `content/`
(catálogo de regras). Contrato entre eles via OpenAPI gerado pelo back-end.

## Alternativas consideradas
- Repositórios separados — overhead de versionar contrato para um time de 1 pessoa.
- Blazor no front — fora do requisito (Angular é obrigatório).
- Controllers MVC — mais cerimônia que Minimal APIs para ~6 endpoints.

## Consequências
- **Positivas**: uma PR muda contrato + consumidor; scripts e gates únicos.
- **Negativas**: duas toolchains no mesmo CI; .NET SDK 10 ainda não instalado na máquina.
- **Follow-ups**: subir Angular para a próxima major quando a 22 sair de suporte ativo.
