import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BONUS_TYPES, KINDS } from '../labels';

/** Legenda fixa dos tipos de bônus e benefícios (F4 AC6). */
@Component({
  selector: 'app-bonus-legend',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <details>
      <summary>Legenda: tipos de bônus</summary>
      <dl>
        @for (t of types; track t[0]) {
          <dt>{{ t[1].icon }} {{ t[1].label }}</dt>
          <dd>{{ t[1].hint }}</dd>
        }
      </dl>
    </details>
  `,
  styles: `
    dl {
      display: grid;
      grid-template-columns: max-content 1fr;
      gap: 0.25rem 1rem;
    }
    dt {
      font-weight: 700;
    }
    dd {
      margin: 0;
      color: var(--muted);
    }
  `,
})
export class BonusLegendComponent {
  protected readonly types = [
    ...Object.entries(BONUS_TYPES),
    ...Object.entries(KINDS).filter(([k]) => k !== 'proficiency'),
  ];
}
