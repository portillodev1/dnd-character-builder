import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { SheetBonus, SheetTrait } from '../../core/api/models';
import { ABILITY_NAMES, bonusType, kind, optionLabel, signed } from '../../shared/labels';
import { BonusBreakdownComponent } from '../../shared/ui/bonus-breakdown';
import { BonusLegendComponent } from '../../shared/ui/bonus-legend';
import { TraitCardComponent } from '../../shared/ui/trait-card';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { StepNavComponent } from '../wizard/step-nav';

interface BonusGroup {
  title: string;
  items: string[];
}

/** Passo 5 — ficha no layout da oficial, com todos os bônus explicados (spec F4). */
@Component({
  selector: 'app-sheet-step',
  imports: [BonusBreakdownComponent, BonusLegendComponent, TraitCardComponent, StepNavComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Ficha</h2>
    <app-bonus-legend />

    @if (store.warnings().length > 0) {
      <section aria-labelledby="pending" class="pending">
        <h3 id="pending">Ainda falta escolher</h3>
        <ul>
          @for (w of store.warnings(); track $index) {
            <li>{{ w }}</li>
          }
        </ul>
      </section>
    }

    @if (store.sheet(); as s) {
      <div class="sheet">
        <section aria-labelledby="abilities" class="abilities">
          <h3 id="abilities">Atributos</h3>
          @for (a of abilities(); track a.id) {
            <div class="ability">
              <span class="name">{{ a.name }}</span>
              <app-bonus-breakdown
                [label]="'Modificador de ' + a.name"
                [total]="a.modifier.total"
                [parts]="a.modifier.parts"
                [withSign]="true"
              />
              <app-bonus-breakdown
                [label]="a.name"
                [total]="a.score.total"
                [parts]="a.score.parts"
              />
            </div>
          }
        </section>

        <section aria-labelledby="numbers">
          <h3 id="numbers">Números principais</h3>
          <dl>
            @if (s.proficiencyBonus; as p) {
              <dt>Bônus de Proficiência</dt>
              <dd>
                <app-bonus-breakdown
                  label="Bônus de Proficiência"
                  [total]="p.total"
                  [parts]="p.parts"
                  [withSign]="true"
                />
              </dd>
            }
            @if (s.hitPoints; as hp) {
              <dt>Pontos de Vida máximos</dt>
              <dd>
                <app-bonus-breakdown label="Pontos de Vida" [total]="hp.total" [parts]="hp.parts" />
              </dd>
            }
            @if (s.passivePerception; as pp) {
              <dt>Sabedoria Passiva (Percepção)</dt>
              <dd>
                <app-bonus-breakdown
                  label="Sabedoria Passiva (Percepção)"
                  [total]="pp.total"
                  [parts]="pp.parts"
                />
              </dd>
            }
            @if (s.speed; as sp) {
              <dt>Deslocamento</dt>
              <dd>
                <app-bonus-breakdown
                  label="Deslocamento"
                  [total]="sp.total"
                  [parts]="sp.parts"
                  unit="m"
                />
              </dd>
            }
          </dl>

          <h3>Salvaguardas</h3>
          <ul class="plain">
            @for (t of saves(); track t.id) {
              <li>
                <span class="dot" [class.on]="t.proficient" aria-hidden="true"></span>
                <app-bonus-breakdown
                  [label]="'Salvaguarda de ' + t.name"
                  [total]="t.total"
                  [parts]="t.parts"
                  [withSign]="true"
                />
                {{ t.name }}
              </li>
            }
          </ul>
        </section>

        <section aria-labelledby="skills">
          <h3 id="skills">Perícias</h3>
          <ul class="plain">
            @for (k of skills(); track k.id) {
              <li>
                <span
                  class="dot"
                  [class.on]="k.proficiency === 'proficient' || k.proficiency === 'expertise'"
                  aria-hidden="true"
                ></span>
                @if (k.total !== null && k.parts) {
                  <app-bonus-breakdown
                    [label]="k.name"
                    [total]="k.total"
                    [parts]="k.parts"
                    [withSign]="true"
                  />
                } @else {
                  —
                }
                {{ k.sheetLabel }}
              </li>
            }
          </ul>
        </section>
      </div>

      <section aria-labelledby="my-bonuses">
        <h3 id="my-bonuses">Meus bônus</h3>
        @for (g of groups(); track g.title) {
          <h4>{{ g.title }}</h4>
          <ul>
            @for (item of g.items; track $index) {
              <li>{{ item }}</li>
            }
          </ul>
        }
      </section>

      <section aria-labelledby="features" class="cards">
        <h3 id="features">Características de classe</h3>
        @for (f of s.features; track f.id) {
          <app-trait-card
            [name]="f.name"
            [kind]="f.kind"
            [description]="f.description"
            [page]="f.source.page"
            [level]="f.level"
            [scaling]="f.scalingValue"
          />
        }
      </section>

      <section aria-labelledby="traits" class="cards">
        <h3 id="traits">Traços raciais</h3>
        @for (t of s.traits; track t.originName + t.id) {
          <app-trait-card
            [name]="t.name + ' (' + t.originName + ')'"
            [kind]="t.kind"
            [description]="t.description"
            [page]="t.source.page"
          />
        }
      </section>
    } @else {
      <p>Corrija os erros dos passos anteriores para ver a ficha.</p>
    }

    <app-step-nav step="ficha" />
  `,
  styles: `
    app-bonus-legend {
      display: block;
      margin-bottom: 1rem;
    }
    .sheet {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(16rem, 1fr));
      gap: 1rem;
      align-items: start;
    }
    .sheet > section {
      padding: 0 1rem 1rem;
      border: var(--rule);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .sheet h3 {
      margin-top: 1rem;
      color: var(--accent);
    }
    .ability {
      display: flex;
      gap: 0.5rem;
      align-items: center;
      padding: 0.35rem 0;
      border-bottom: 1px solid var(--border);
    }
    .ability:last-child {
      border-bottom: 0;
    }
    .ability .name {
      min-width: 7rem;
      font-weight: 600;
    }
    dl {
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 0.5rem;
      align-items: center;
    }
    dd {
      margin: 0;
    }
    .plain {
      margin: 0;
      padding: 0;
      list-style: none;
    }
    .plain li {
      display: flex;
      gap: 0.5rem;
      align-items: center;
      padding: 0.15rem 0;
    }
    .dot {
      flex: none;
      display: inline-block;
      width: 0.8rem;
      height: 0.8rem;
      border: 2px solid var(--ink);
      border-radius: 50%;
    }
    .dot.on {
      background: var(--accent);
      border-color: var(--accent);
    }
    .cards {
      display: grid;
      gap: 0.75rem;
    }
    .cards h3 {
      margin-bottom: 0;
    }
    .pending {
      padding: 0.25rem 1rem;
      border-left: 3px double var(--warning);
      border-radius: var(--radius-sm);
      background: var(--warning-soft);
    }
    .pending h3 {
      margin-top: 0.5rem;
      color: var(--warning);
    }
  `,
})
export class SheetStepComponent {
  protected readonly store = inject(CharacterDraftStore);

  protected readonly abilities = computed(() =>
    Object.entries(this.store.sheet()?.abilities ?? {}).map(([id, a]) => ({ id, ...a })),
  );
  protected readonly saves = computed(() =>
    Object.entries(this.store.sheet()?.savingThrows ?? {}).map(([id, t]) => ({
      id,
      name: ABILITY_NAMES[id] ?? id,
      ...t,
    })),
  );
  protected readonly skills = computed(() =>
    Object.entries(this.store.sheet()?.skills ?? {}).map(([id, k]) => ({ id, ...k })),
  );

  /** "Meus bônus" agrupado por origem: Raça, Sub-raça, Classe (nível N), Escolhas (F4 AC4–AC5). */
  protected readonly groups = computed<BonusGroup[]>(() => {
    const sheet = this.store.sheet();
    const draft = this.store.draft();
    if (!sheet) {
      return [];
    }
    const byOrigin = (origin: string) => [
      ...sheet.bonuses.filter((b) => b.origin === origin).map(bonusLine),
      ...sheet.traits.filter((t) => t.origin === origin).map(traitLine),
    ];
    const race =
      sheet.bonuses.find((b) => b.origin === 'race')?.originName ??
      sheet.traits.find((t) => t.origin === 'race')?.originName;
    const subrace =
      sheet.bonuses.find((b) => b.origin === 'subrace')?.originName ??
      sheet.traits.find((t) => t.origin === 'subrace')?.originName;
    const groups: BonusGroup[] = [
      { title: `Raça${race ? ` — ${race}` : ''}`, items: byOrigin('race') },
      { title: `Sub-raça${subrace ? ` — ${subrace}` : ''}`, items: byOrigin('subrace') },
      {
        title: `Classe (nível ${draft.level ?? 1})`,
        items: sheet.features.map(
          (f) =>
            `${f.name}${f.scalingValue ? ` (${f.scalingValue})` : ''} · ${kind(f.kind).label} · nível ${f.level}`,
        ),
      },
      { title: 'Escolhas', items: byOrigin('choice') },
    ];
    return groups.filter((g) => g.items.length > 0);
  });
}

function bonusLine(b: SheetBonus): string {
  const target = b.target === 'hp-per-level' ? 'PV por nível' : optionLabel(b.target);
  // Escolhas dizem de quem é a escolha (Meio-Elfo, Ladino, Antecedente).
  const from =
    b.origin === 'choice' ? `${b.label ?? ''} (${b.originName})` : (b.label ?? b.originName);
  return `${signed(b.value)} ${target} · ${bonusType(b.bonusType).label} · ${from}`;
}

function traitLine(t: SheetTrait): string {
  const k = kind(t.kind);
  const from = t.origin === 'choice' ? ` (${t.originName})` : '';
  return `${t.name} · ${k.label}${from}${t.kind === 'advantage' ? ' (role 2d20 e fique com o maior)' : ''}`;
}
