# docs/adr/ — Architecture Decision Records (para HUMANOS)

Uma decisão por arquivo (`ADR-0001-...md`). São imutáveis: para reverter, crie um novo
ADR com status "substitui ADR-XXXX". O AGENTS.md apenas **aponta** para cá.

Gatilho: dependência nova ou decisão de arquitetura (BOUNDARIES do AGENTS.md).
Modelo: [ADR.template.md](ADR.template.md).

| ADR | Decisão | Status |
|---|---|---|
| [0001](ADR-0001-monorepo-angular-dotnet.md) | Monorepo com Angular 22 + .NET 10 LTS | aceito |
| [0002](ADR-0002-stateless-api-rules-in-domain.md) | API stateless, sem banco; cálculo só no Domain | aceito (persistência substituída pelo 0006) |
| [0003](ADR-0003-ruleset-as-versioned-json.md) | Catálogo de regras como JSON versionado com schema | aceito |
| [0004](ADR-0004-content-licensing.md) | Conteúdo: SRD 5.1 + resumos próprios; PDF não versionado | aceito |
| [0005](ADR-0005-json-schema-validator.md) | JsonSchema.Net para validar `content/` (isento da taxa OSMF) | aceito |
| [0006](ADR-0006-character-sheets-as-json-files.md) | Fichas salvas como arquivos JSON no servidor (sem banco) | aceito |
| [0007](ADR-0007-openapi-typescript.md) | Tipos do front gerados do OpenAPI (openapi-typescript) | aceito |
| [0008](ADR-0008-fontsource-fonts.md) | Fontes de época empacotadas (@fontsource, OFL) | aceito |
| [0009](ADR-0009-playwright-axe-e2e.md) | E2E com Playwright + axe-core (API e front isolados) | aceito |
