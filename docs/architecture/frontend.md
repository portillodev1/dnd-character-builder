# Front-end (Angular 22 · Node 24 LTS)

SPA em formato de **wizard** guiado. Regras para a IA: `rules/frontend-angular.md`.

## Layout
```txt
frontend/
  angular.json · package.json · eslint.config.js · .prettierrc
  src/app/
    app.config.ts            # provideZonelessChangeDetection, provideHttpClient, rotas
    app.routes.ts            # /fichas (lista) e /fichas/:id/<passo> (wizard), lazy
    core/
      api/generated/         # tipos/cliente gerados do OpenAPI (não editar)
      api/ruleset.service.ts # catálogo (raças, classes)
      api/sheet.service.ts   # compute + CRUD de fichas salvas (ADR-0006)
    state/
      character-draft.store.ts  # signals; ficha aberta, salva via PUT com debounce
    features/
      my-sheets/     # "Minhas fichas": listar, nova, abrir, renomear, apagar (critério 5)
      wizard/        # shell da ficha aberta (nome, "Salvo às", 409), passos, step-nav, choice-picker
      race/          # passo 1 — escolher raça/sub-raça, ver traços (critério 1)
      class/         # passo 2 — classe + nível, ver características (critério 2)
      abilities/     # passo 3 — valores de atributo (padrão / compra / manual)
      skills-saves/  # passo 4 — perícias e salvaguardas com "como calcular" (critério 3)
      sheet/         # passo 5 — ficha final no layout oficial + bônus tipados (critério 4)
    shared/ui/
      bonus-breakdown/  # popover: parcelas, tipo de bônus, página do LdJ
      stepper/ · trait-card/ · glossary-term/
```

## Fluxo do wizard
```txt
Raça ─► Classe+Nível ─► Atributos ─► Perícias/Salvaguardas ─► Ficha
  └─ cada passo grava no draft store ─► debounce ─► PUT /character-sheets/{id} ─► store.sheet
```
- O **store** guarda só escolhas do usuário; a ficha derivada é sempre a última resposta da API.
- Voltar a um passo invalida escolhas dependentes (ex.: trocar classe limpa perícias de classe).

## UX para iniciante
- Cada passo tem um bloco "O que é isso?" com resumo próprio + página do LdJ.
- `glossary-term` mostra o termo da ficha e o sinônimo do livro (ex.: Salvaguarda = Teste de Resistência).
- Cores/ícones por **tipo de bônus** (legenda fixa em `docs/domain/bonus-types.md`).
- Ficha final espelha as seções da ficha oficial (atributos à esquerda, salvaguardas,
  perícias, Sabedoria Passiva).

## Testes
- Vitest (runner padrão do Angular) para componentes e store.
- Playwright (fase 2) para o fluxo completo do wizard.
