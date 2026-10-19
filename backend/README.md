# backend/ — API .NET 10 LTS

Estrutura reservada; projetos serão criados na **Fase 1** do `ROADMAP.md`.
Arquitetura: [docs/architecture/backend.md](../docs/architecture/backend.md) ·
Regras para a IA: [rules/backend-dotnet.md](../rules/backend-dotnet.md).

| Pasta | Projeto | Papel |
|---|---|---|
| `src/DndBuilder.Domain` | classlib | Motor de regras puro (fórmulas, `DerivedValue`) |
| `src/DndBuilder.Application` | classlib | Casos de uso + porta `IRulesetCatalog` |
| `src/DndBuilder.Infrastructure` | classlib | Catálogo `content/` embarcado + validação de schema |
| `src/DndBuilder.Api` | web (Minimal API) | Endpoints `/api/v1`, OpenAPI, ProblemDetails |
| `tests/DndBuilder.Domain.Tests` | xUnit v3 | Testes de tabela das fórmulas |
| `tests/DndBuilder.Ruleset.Tests` | xUnit v3 | Schema + integridade do catálogo |
| `tests/DndBuilder.Api.Tests` | xUnit v3 | Contrato HTTP + teste de arquitetura |
