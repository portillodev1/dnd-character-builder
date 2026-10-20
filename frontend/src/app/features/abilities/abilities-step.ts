import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ABILITY_NAMES } from '../../shared/labels';
import { BonusBreakdownComponent } from '../../shared/ui/bonus-breakdown';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { StepNavComponent } from '../wizard/step-nav';

interface Method {
  id: 'standard-array' | 'point-buy' | 'manual';
  label: string;
  help: string;
  min: number;
  max: number;
}

/**
 * Faixas só para os campos do formulário; quem valida (e explica o erro) é a API
 * (BaseScoreRules no Domain, LdJ p. 13).
 */
const METHODS: readonly Method[] = [
  {
    id: 'standard-array',
    label: 'Conjunto padrão',
    help: 'Distribua 15, 14, 13, 12, 10 e 8, um em cada atributo.',
    min: 8,
    max: 15,
  },
  {
    id: 'point-buy',
    label: 'Compra de pontos',
    help: 'Gaste 27 pontos; cada valor vai de 8 a 15 e os mais altos custam mais.',
    min: 8,
    max: 15,
  },
  {
    id: 'manual',
    label: 'Manual (rolagem)',
    help: 'Role 4d6, descarte o menor dado e anote cada total (3 a 18).',
    min: 3,
    max: 18,
  },
];
/** Valores do conjunto padrão (LdJ p. 13). */
const STANDARD_ARRAY = [15, 14, 13, 12, 10, 8];
const ABILITIES = Object.keys(ABILITY_NAMES);

/** Passo 3 — valores base dos atributos; totais e modificadores vêm da API com a conta. */
@Component({
  selector: 'app-abilities-step',
  imports: [BonusBreakdownComponent, StepNavComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Atributos</h2>
    <p class="help">
      Os seis atributos medem o que seu personagem faz bem. Escolha um método para os valores base;
      depois a raça soma seus aumentos (LdJ p. 13 e p. 175).
    </p>

    <fieldset>
      <legend>Método</legend>
      @for (m of methods; track m.id) {
        <label>
          <input
            type="radio"
            name="method"
            [value]="m.id"
            [checked]="m.id === method().id"
            (change)="pickMethod(m.id)"
          />
          {{ m.label }}
        </label>
      }
      <p>{{ method().help }}</p>
    </fieldset>

    @for (e of formErrors(); track $index) {
      <p role="alert" class="error">{{ e }}</p>
    }

    <table>
      <thead>
        <tr>
          <th scope="col">Atributo</th>
          <th scope="col">Valor base</th>
          <th scope="col">Valor final</th>
          <th scope="col">Modificador</th>
        </tr>
      </thead>
      <tbody>
        @for (a of abilities; track a) {
          <tr>
            <th scope="row">
              <label [for]="'base-' + a">{{ name(a) }}</label>
            </th>
            <td>
              @if (method().id === 'standard-array') {
                <select [id]="'base-' + a" (change)="setBase(a, $event)">
                  <option value="" [selected]="base(a) === null">—</option>
                  @for (v of standardArray; track v) {
                    <option [value]="v" [selected]="base(a) === v" [disabled]="usedElsewhere(a, v)">
                      {{ v }}
                    </option>
                  }
                </select>
              } @else {
                <input
                  [id]="'base-' + a"
                  type="number"
                  [min]="method().min"
                  [max]="method().max"
                  [value]="base(a) ?? ''"
                  (change)="setBase(a, $event)"
                />
              }
            </td>
            @if (sheetAbility(a); as s) {
              <td>
                <app-bonus-breakdown
                  [label]="name(a)"
                  [total]="s.score.total"
                  [parts]="s.score.parts"
                />
              </td>
              <td>
                <app-bonus-breakdown
                  [label]="'Modificador de ' + name(a)"
                  [total]="s.modifier.total"
                  [parts]="s.modifier.parts"
                  [withSign]="true"
                />
              </td>
            } @else {
              <td>—</td>
              <td>—</td>
            }
          </tr>
        }
      </tbody>
    </table>

    <app-step-nav
      step="atributos"
      [blockedReason]="complete() ? null : 'Defina os seis valores base.'"
    />
  `,
  styles: `
    fieldset {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    fieldset label {
      display: inline-flex;
      gap: 0.4rem;
      align-items: center;
      padding: 0.35rem 0.8rem;
      border: 1px solid var(--border);
      border-radius: var(--radius-sm);
      cursor: pointer;
    }
    fieldset label:has(:checked) {
      border-color: var(--accent);
      background: var(--accent-soft);
      font-weight: 600;
    }
    fieldset p {
      width: 100%;
      margin: 0.25rem 0 0;
      color: var(--muted);
    }
    table {
      margin-top: 1rem;
    }
    tbody th {
      font-weight: 600;
    }
    td input {
      width: 5rem;
    }
    .error {
      color: var(--danger);
    }
  `,
})
export class AbilitiesStepComponent {
  private readonly store = inject(CharacterDraftStore);

  protected readonly methods = METHODS;
  protected readonly standardArray = STANDARD_ARRAY;
  protected readonly abilities = ABILITIES;
  protected readonly method = computed(
    () => METHODS.find((m) => m.id === this.store.draft().abilityMethod) ?? METHODS[0],
  );
  protected readonly complete = computed(() => ABILITIES.every((a) => this.base(a) !== null));
  protected readonly formErrors = computed(() =>
    Object.entries(this.store.errors())
      .filter(([k]) => k.startsWith('baseAbilities') || k === 'abilityMethod')
      .flatMap(([, v]) => v),
  );

  protected name(a: string): string {
    return ABILITY_NAMES[a] ?? a;
  }

  protected base(a: string): number | null {
    return this.store.draft().baseAbilities?.[a] ?? null;
  }

  protected sheetAbility(a: string) {
    return this.store.sheet()?.abilities[a] ?? null;
  }

  protected usedElsewhere(a: string, v: number): boolean {
    return ABILITIES.some((other) => other !== a && this.base(other) === v);
  }

  protected pickMethod(id: Method['id']): void {
    // Os valores de um método não valem para outro: começa do zero.
    this.store.update({ abilityMethod: id, baseAbilities: {} });
  }

  protected setBase(a: string, event: Event): void {
    const raw = (event.target as HTMLInputElement | HTMLSelectElement).value;
    this.store.update((d) => {
      const next = { ...(d.baseAbilities ?? {}) };
      if (raw === '') {
        delete next[a];
      } else {
        next[a] = Number(raw);
      }
      return { ...d, abilityMethod: this.method().id, baseAbilities: next };
    });
  }
}
