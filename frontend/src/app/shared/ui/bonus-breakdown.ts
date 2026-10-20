import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { BonusPart } from '../../core/api/models';
import { bonusType, signed } from '../labels';

/** Ordem das parcelas: base → atributo → proficiência → extras (F4 AC2). */
const ORDER = [
  'base-score',
  'ability-score-increase',
  'ability-modifier',
  'proficiency-bonus',
  'proficiency',
  'expertise',
  'half-proficiency',
];
const rank = (t: string) => {
  const i = ORDER.indexOf(t);
  return i === -1 ? ORDER.length : i;
};

let nextId = 0;

/**
 * Número da ficha que abre a conta (F4 AC1–AC2): botão focável; Enter/Espaço/clique alternam
 * o painel com as parcelas, cada uma com valor, rótulo, tipo (texto + ícone) e página do LdJ.
 */
@Component({
  selector: 'app-bonus-breakdown',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button
      type="button"
      class="total"
      [attr.aria-expanded]="open()"
      [attr.aria-controls]="panelId"
      [attr.aria-label]="label() + ': ' + shown() + '. Ver a conta'"
      (click)="toggle()"
      (keydown.enter)="key($event)"
      (keydown.space)="key($event)"
    >
      {{ shown() }}
    </button>
    @if (open()) {
      <div class="panel" role="region" [id]="panelId" [attr.aria-label]="'Conta de ' + label()">
        <p class="title">{{ label() }} = {{ shown() }}</p>
        <ul>
          @for (p of ordered(); track $index) {
            <li [attr.data-type]="p.bonusType">
              <span class="value">{{ sign(p.value) }}</span>
              <span class="label">{{ p.label }}</span>
              <span class="type">{{ type(p.bonusType).icon }} {{ type(p.bonusType).label }}</span>
              <span class="page">LdJ p. {{ p.source.page }}</span>
            </li>
          }
        </ul>
      </div>
    }
  `,
  styles: `
    :host {
      display: inline-block;
      position: relative;
    }
    .total {
      min-width: 2.5rem;
      font: inherit;
      font-weight: 700;
      cursor: pointer;
    }
    .total:focus-visible {
      outline: 3px solid #1a5fb4;
      outline-offset: 2px;
    }
    .panel {
      position: absolute;
      z-index: 10;
      min-width: 18rem;
      padding: 0.5rem 0.75rem;
      background: #fff;
      border: 1px solid #555;
      border-radius: 0.25rem;
      box-shadow: 0 2px 6px #0003;
    }
    ul {
      margin: 0;
      padding: 0;
      list-style: none;
    }
    li {
      display: grid;
      grid-template-columns: 3rem 1fr;
      gap: 0 0.5rem;
      padding: 0.25rem 0;
    }
    .value {
      font-weight: 700;
      grid-row: span 2;
    }
    .type,
    .page {
      font-size: 0.85em;
      color: #444;
    }
    .title {
      margin: 0 0 0.25rem;
      font-weight: 700;
    }
  `,
})
export class BonusBreakdownComponent {
  readonly label = input.required<string>();
  readonly total = input.required<number>();
  readonly parts = input.required<readonly BonusPart[]>();
  /** Mostra com sinal (+3) — modificadores, perícias, salvaguardas. */
  readonly withSign = input(false);

  protected readonly open = signal(false);
  protected readonly panelId = `bonus-breakdown-${nextId++}`;
  protected readonly shown = computed(() =>
    this.withSign() ? signed(this.total()) : `${this.total()}`,
  );
  protected readonly ordered = computed(() =>
    [...this.parts()].sort((a, b) => rank(a.bonusType) - rank(b.bonusType)),
  );
  protected readonly sign = signed;
  protected readonly type = bonusType;

  protected toggle(): void {
    this.open.update((o) => !o);
  }

  protected key(event: Event): void {
    event.preventDefault(); // evita o clique nativo duplicado
    this.toggle();
  }
}
