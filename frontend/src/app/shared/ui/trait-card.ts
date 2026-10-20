import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { kind } from '../labels';

/** Traço de raça ou característica de classe: nome, tipo, resumo e página (F1 AC4, F2 AC4). */
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
        <span class="page">· LdJ p. {{ page() }}</span>
      </p>
      <p>{{ description() }}</p>
    </article>
  `,
  styles: `
    article {
      border: 1px solid #ccc;
      border-radius: 0.25rem;
      padding: 0.5rem 0.75rem;
    }
    h4 {
      margin: 0;
    }
    .meta {
      margin: 0.25rem 0;
      font-size: 0.85em;
      color: #444;
    }
    p {
      margin: 0.25rem 0 0;
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

  protected readonly type = computed(() => kind(this.kind()));
}
