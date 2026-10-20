import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Choice } from '../../core/api/models';
import { optionLabel } from '../../shared/labels';

/** Escolha pendente do catálogo (ex.: Meio-Elfo +1 em dois atributos): marque `count` opções. */
@Component({
  selector: 'app-choice-picker',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <fieldset>
      <legend>
        {{ choice().name }} — escolha {{ choice().count }}
        <span class="counter">({{ picked().length }} de {{ choice().count }})</span>
      </legend>
      @for (option of choice().from; track option) {
        <label>
          <input
            type="checkbox"
            [checked]="picked().includes(option)"
            [disabled]="full() && !picked().includes(option)"
            (change)="toggled.emit(option)"
          />
          {{ label(option) }}
        </label>
      }
      <p class="page">LdJ p. {{ choice().source.page }}</p>
    </fieldset>
  `,
  styles: `
    fieldset {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      border-color: var(--accent);
    }
    label {
      display: inline-flex;
      gap: 0.4rem;
      align-items: center;
      padding: 0.3rem 0.7rem;
      border: 1px solid var(--border);
      border-radius: 999px;
      cursor: pointer;
    }
    label:has(:checked) {
      border-color: var(--accent);
      background: var(--accent-soft);
    }
    label:has(:disabled) {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .counter,
    .page {
      font-weight: 400;
      font-size: 0.9em;
      color: var(--muted);
      width: 100%;
      margin: 0;
    }
  `,
})
export class ChoicePickerComponent {
  readonly choice = input.required<Choice>();
  readonly picked = input.required<readonly string[]>();
  /** Opção marcada/desmarcada; quem decide a lista nova é o store (sempre o estado mais recente). */
  readonly toggled = output<string>();

  protected readonly full = computed(() => this.picked().length >= this.choice().count);
  protected readonly label = optionLabel;
}
