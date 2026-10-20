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
      min-width: 2.75rem;
      padding: 0.2rem 0.5rem;
      border: 1px solid var(--control-border);
      border-bottom-width: 2px;
      border-radius: var(--radius-sm);
      background: var(--surface);
      font: inherit;
      font-weight: 600;
      font-variant-numeric: tabular-nums;
      cursor: pointer;
    }
    .total:hover {
      border-color: var(--accent);
      background: var(--accent-soft);
    }
    .total[aria-expanded='true'] {
      border-color: var(--accent);
      background: var(--accent);
      color: var(--accent-ink);
    }
    .total:focus-visible {
      outline: 3px solid var(--focus);
      outline-offset: 2px;
    }
    .panel {
      position: absolute;
      z-index: 10;
      top: calc(100% + 0.35rem);
      left: 0;
      min-width: 18rem;
      max-width: min(24rem, calc(100vw - 2rem));
      padding: 0.6rem 0.85rem;
      background: var(--surface);
      border: 1px solid var(--control-border);
      border-radius: var(--radius);
      box-shadow: var(--shadow);
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
      padding: 0.35rem 0;
      border-top: 1px solid var(--border);
    }
    .value {
      font-weight: 600;
      font-variant-numeric: tabular-nums;
      grid-row: span 3;
      color: var(--accent);
    }
    .type,
    .page {
      font-size: 0.85em;
      color: var(--muted);
    }
    .title {
      margin: 0 0 0.35rem;
      font-family: var(--font-display);
      font-size: 1.15rem;
    }
  `,
})
export class BonusBreakdownComponent {
  readonly label = input.required<string>();
  readonly total = input.required<number>();
  readonly parts = input.required<readonly BonusPart[]>();
  /** Mostra com sinal (+3) — modificadores, perícias, salvaguardas. */
  readonly withSign = input(false);
  /** Unidade após o total (ex.: "m" no deslocamento). */
  readonly unit = input('');

  protected readonly open = signal(false);
  protected readonly panelId = `bonus-breakdown-${nextId++}`;
  protected readonly shown = computed(() => {
    const n = this.withSign() ? signed(this.total()) : this.total().toLocaleString('pt-BR');
    return this.unit() ? `${n} ${this.unit()}` : n;
  });
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
