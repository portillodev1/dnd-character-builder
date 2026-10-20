# Spec: D&D Character Builder (projeto)

Skill: `spec-driven-development` · Fase: **Specify ✅ → Plan ✅ → Tasks ☐** (aguarda revisão).

## Objective
Jogador **iniciante** de D&D 5e cria a ficha passo a passo e entende **de onde vem cada
número**. Sucesso = os 5 critérios de pronto (ver `ROADMAP.md`), cada um detalhado numa
feature spec em `.specs/features/`.

## Assumptions (corrija se errado)
1. Web app responsivo (desktop primeiro); sem app mobile nativo.
2. Sem login e sem banco; fichas salvas como arquivos JSON no servidor, só escolhas (ADR-0006).
3. Idioma único pt-BR; termos da ficha oficial (glossário).
4. Escopo de regras: raças/sub-raças e classes base do LdJ, níveis 1–20; sem multiclasse.
5. Antecedente no MVP = só marcar 2 perícias extras (sem catálogo de antecedentes).
6. Aumento de atributo por nível (ASI) fora do MVP; atributos = base + raça.
7. Uso local/dev; deploy definido depois.

## Tech Stack
Angular 22 · Node 24 LTS · TypeScript strict · Vitest · .NET 10 LTS · ASP.NET Core
Minimal APIs · xUnit v3 · JSON Schema · OpenAPI nativo. Detalhe: `docs/architecture/`.

## Commands
```txt
Bootstrap: bash scripts/bootstrap.sh
Build:     bash scripts/build.sh
Test:      bash scripts/test.sh [--back|--front] [--filter <p>] [--watch]
Lint:      bash scripts/lint.sh [--fix]
Gate:      bash scripts/verify.sh --strict
Dev (fase 1): dotnet watch --project backend/src/DndBuilder.Api · cd frontend && npx ng serve
```

## Project Structure
```txt
backend/   → .NET (src/Domain, Application, Infrastructure, Api · tests/*)
frontend/  → Angular (core/, state/, features/<passo>/, shared/ui/)
content/   → catálogo JSON (schemas/, races/, classes/, skills.json)
docs/      → arquitetura, domínio, ADRs, harness (humanos)
rules/     → regras testáveis para a IA
scripts/   → gates e comandos repetíveis
.specs/    → specs (projeto + features)
```

## Code Style
Ver `rules/backend-dotnet.md` e `rules/frontend-angular.md` (exemplos reais lá).

## Testing Strategy
Ver `rules/tests.md`. Ênfase: testes de tabela das fórmulas no `Domain` + validação do catálogo.

## Boundaries
Ver BOUNDARIES do `AGENTS.md` (fonte única).

## Success Criteria (projeto)
- [ ] F1–F4 com todos os critérios de aceite verdes (testes + demo no wizard).
- [ ] `scripts/verify.sh --strict` sai 0 no CI.
- [ ] Cobertura do `Domain` ≥ 90%.
- [ ] Nenhum valor numérico na ficha sem breakdown (teste de contrato).

## Resolved Questions (2026-10-19)
1. **Conteúdo fora do SRD 5.1**: incluir, só nome + mecânica resumida (ADR-0004 aceito).
2. **Métodos de atributo**: os três no MVP — conjunto padrão, compra de pontos (27), manual.
3. **Antecedentes**: só marcar 2 perícias (assumption 5 confirmada).
4. **Deploy**: só local por enquanto (assumption 7).
5. **Exportar ficha**: pós-MVP.
