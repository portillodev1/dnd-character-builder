# rules/frontend-angular.md — carregada em: código Angular

Stack: **Angular 22**, Node **24 LTS**, TypeScript strict. Arquitetura (humanos):
`docs/architecture/frontend.md`. Para API atual do framework use o **context7 MCP**.

## Regras
- Componentes **standalone**, `ChangeDetectionStrategy.OnPush`, app **zoneless**.
- Estado com **signals** (`signal`, `computed`, `linkedSignal`); RxJS só na borda HTTP.
- Template com control flow nativo (`@if`, `@for`, `@switch`) — nunca `*ngIf`/`*ngFor`.
- `inject()` em vez de injeção por construtor.
- Organização por **feature** (`features/<passo-do-wizard>/`); `shared/ui` só para
  componentes sem regra de negócio; `core/` para API client, config e interceptors.
- **Nenhuma fórmula de D&D no front.** Valores e breakdowns vêm da API; o front só renderiza.
- Tipos da API **gerados** a partir do OpenAPI do back-end (`core/api/generated/`, não editar à mão).
- Todo número da ficha é clicável/focável e abre o breakdown (`<app-bonus-breakdown>`).
- Acessibilidade: WCAG 2.2 AA — labels, foco visível, navegação por teclado no wizard.
- Textos de UI em pt-BR; terminologia igual à ficha oficial.
- Rotas lazy por feature (`loadComponent`).

## Estrutura (resumo)
```txt
frontend/src/app/
  core/       api client (gerado), http interceptors, config
  state/      character-draft.store.ts (signals; rascunho em localStorage)
  features/   race/ class/ abilities/ skills-saves/ sheet/
  shared/ui/  bonus-breakdown, stepper, tooltip
```

## Exemplo — componente padrão
```ts
@Component({
  selector: 'app-race-traits',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `@for (t of traits(); track t.id) { <app-trait-card [trait]="t" /> }`,
})
export class RaceTraitsComponent {
  readonly traits = input.required<RaceTrait[]>();
}
```
