# ADR-0007: Tipos do front gerados do OpenAPI com openapi-typescript

- **Status**: aceito
- **Data**: 2026-10-20
- **Decisores**: Gabriel Portillo

## Contexto
`rules/frontend-angular.md` exige que os tipos da API venham do OpenAPI do back-end
(`core/api/generated/`, nunca à mão), para o front não divergir do contrato. Angular não gera
tipos de OpenAPI; precisamos de uma ferramenta de desenvolvimento.

## Decisão
Vamos usar **openapi-typescript 7.13.0** (MIT) como `devDependency` do `frontend/`. Ela gera
só tipos (`generated/schema.ts`), sem cliente em runtime: os serviços em `core/api/` continuam
usando o `HttpClient` do Angular.
- O documento `/openapi/v1.json` fica versionado em `frontend/src/app/core/api/openapi.json`.
- `scripts/gen-api.sh` atualiza o snapshot (via teste `OpenApiSnapshotTests` com
  `UPDATE_OPENAPI=1`) e regenera os tipos; o mesmo teste **falha** se o contrato mudar sem
  regenerar (gate no `verify.sh`).
- A Api passa a usar `JsonNumberHandling.Strict`, para o OpenAPI descrever números como
  `integer`/`number` (antes saía `integer | string`).
- O pacote declara peer `typescript ^5`; o projeto usa TypeScript 6, então o `package.json`
  tem um `overrides` apontando para o TypeScript do projeto.

## Alternativas consideradas
- Script próprio de conversão — sem dependência, mas mais código para manter.
- openapi-generator / orval — geram cliente completo (e, no caso do primeiro, exigem Java);
  mais do que precisamos.
- Tipos à mão — divergem do contrato sem aviso.

## Consequências
- **Positivas**: contrato único; mudança no back que quebra o front aparece no build/teste.
- **Negativas / trade-offs**: override de peer dependency; snapshot precisa ser regenerado a
  cada mudança de DTO.
- **Follow-ups**: remover o `overrides` quando o openapi-typescript aceitar TypeScript 6.
