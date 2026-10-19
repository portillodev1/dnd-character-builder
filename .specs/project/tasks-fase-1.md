# Tasks — Fase 1: Scaffold (efêmero)

Fonte: `.specs/project/plan.md` item 1 · Gate de saída: `bash scripts/verify.sh --strict` → 0.
Apagar este arquivo após a Fase 1 aprovada (política de limpeza do AGENTS.md).

- [x] T1 — Solução .NET e props compartilhadas
  - Acceptance: `backend/DndBuilder.slnx`, `Directory.Build.props` (net10.0, Nullable,
    TreatWarningsAsErrors, ImplicitUsings), `Directory.Packages.props` (CPM), `global.json` (SDK 10).
  - Verify: `scripts/build.sh` compila.
  - Files: `backend/*.props`, `backend/global.json`, `backend/DndBuilder.slnx`

- [x] T2 — 4 projetos de src com referências conforme `rules/backend-dotnet.md`
  - Acceptance: Domain sem referências; Application → Domain; Infrastructure → Application, Domain;
    Api → Application, Infrastructure. Api expõe `GET /health` e OpenAPI em `/openapi/v1.json`.
  - Verify: `scripts/build.sh`; `dotnet run` responde `/health` 200.
  - Files: `backend/src/*/*.csproj`, `backend/src/DndBuilder.Api/Program.cs`

- [x] T3 — 3 projetos de teste (xUnit v3) com 1 teste cada
  - Acceptance: Domain.Tests (smoke), Ruleset.Tests (smoke), Api.Tests (`/health` = 200 via
    `WebApplicationFactory`) + teste de arquitetura "Domain não referencia outros assemblies".
  - Verify: `scripts/test.sh --back` verde.
  - Files: `backend/tests/*`

- [x] T4 — App Angular 22
  - Acceptance: `frontend/` via Angular CLI 22 (standalone, zoneless, SCSS, sem SSR, Vitest),
    rotas vazias por passo do wizard, pastas `core/ state/ features/ shared/ui/`.
  - Verify: `scripts/build.sh` e `scripts/test.sh --front` verdes.
  - Files: `frontend/**`

- [x] T5 — Lint/format do front
  - Acceptance: angular-eslint + Prettier configurados; `ng lint` e `prettier --check` passam.
  - Verify: `scripts/lint.sh` verde.
  - Files: `frontend/eslint.config.js`, `frontend/.prettierrc`, `frontend/angular.json`

- [x] T6 — CI GitHub Actions
  - Acceptance: workflow em PR e push na `main` roda `bash scripts/verify.sh --strict`
    com .NET 10 e Node 24.
  - Verify: YAML válido; roda no GitHub após o próximo push (aprovado pelo usuário).
  - Files: `.github/workflows/verify.yml`

- [x] T7 — Gate estrito + atualização de estado
  - Acceptance: `scripts/verify.sh --strict` sai 0; `STATE.md`, `ROADMAP.md` e `HARNESS.md`
    atualizados (sensores planejados → ✅ onde instalados).
  - Verify: `bash scripts/verify.sh --strict`
