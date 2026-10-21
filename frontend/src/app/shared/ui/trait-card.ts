import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { kind } from '../labels';

/** Traço de raça ou característica de classe: nome, tipo, resumo e página (F1 AC4, F2 AC4, F9 AC3). */
@Component({
  selector: 'app-trait-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article>
      <h4>
        {{ name() }}
        @if (level(); as l) {
          <small>· nível {{ l }}</small>
        }
      </h4>
      <p class="meta">
        <span class="kind" [title]="type().hint">{{ type().icon }} {{ type().label }}</span>
        @if (scaling(); as s) {
          <span class="scaling"
            >· agora: <strong>{{ s }}</strong></span
          >
        }
        @if (origin(); as o) {
          <span class="origin">· {{ o }}</span>
        }
        <span class="page">· LdJ p. {{ page() }}</span>
      </p>
      <p>{{ description() }}</p>
      @if (manual()) {
        <p class="manual">Ainda não aplicado nesta ficha — anote à mão.</p>
      }
    </article>
  `,
  styles: `
    article {
      padding: 0.75rem 1rem;
      border: 1px solid var(--border);
      border-left: 3px double var(--accent);
      border-radius: var(--radius);
      background: var(--surface);
    }
    h4 {
      margin: 0;
    }
    small {
      font-family: var(--font-body);
      font-weight: 400;
    }
    .meta {
      margin: 0.25rem 0;
      font-size: 0.85em;
      color: var(--muted);
    }
    p {
      margin: 0.25rem 0 0;
    }
    .manual {
      display: inline-block;
      margin-top: 0.4rem;
      padding: 0.1rem 0.5rem;
      border-left: 3px double var(--warning);
      background: var(--warning-soft);
      font-size: 0.9em;
      font-style: italic;
    }
  `,
})
export class TraitCardComponent {
  readonly name = input.required<string>();
  readonly kind = input.required<string>();
  readonly description = input.required<string>();
  readonly page = input.required<number>();
  readonly level = input<number | null>(null);
  readonly scaling = input<string | null>(null);
  /** O app ainda não aplica a característica (ASI, arquétipo) — vem da API (F9 AC3). */
  readonly manual = input(false);
  /** Dono da característica quando não é a classe base (nome do arquétipo — spec F12). */
  readonly origin = input<string | null>(null);

  protected readonly type = computed(() => kind(this.kind()));
}
