import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { Race, SheetBonus, SheetTrait } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import {
  ABILITY_NAMES,
  SKILL_NAMES,
  bonusType,
  kind,
  optionLabel,
  signed,
} from '../../shared/labels';
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
            @if (race(); as r) {
              <dt>Deslocamento</dt>
              <dd>
                {{ r.speed.meters }} m
                <small>({{ r.name }}, LdJ p. {{ r.speed.source.page }})</small>
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
    .sheet {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(14rem, 1fr));
      gap: 1rem;
    }
    .ability {
      display: flex;
      gap: 0.5rem;
      align-items: center;
      padding: 0.25rem 0;
    }
    .ability .name {
      min-width: 7rem;
    }
    dl {
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 0.25rem 0.5rem;
    }
    dd {
      margin: 0;
    }
    .plain {
      padding: 0;
      list-style: none;
    }
    .plain li {
      display: flex;
      gap: 0.5rem;
      align-items: center;
      padding: 0.1rem 0;
    }
    .dot {
      display: inline-block;
      width: 0.8rem;
      height: 0.8rem;
      border: 2px solid #333;
      border-radius: 50%;
    }
    .dot.on {
      background: #333;
    }
    .cards {
      display: grid;
      gap: 0.5rem;
    }
    .pending {
      border-left: 4px solid #9c6500;
      padding-left: 0.75rem;
    }
  `,
})
export class SheetStepComponent {
  private readonly ruleset = inject(RulesetService);
  protected readonly store = inject(CharacterDraftStore);
  protected readonly race = signal<Race | null>(null);

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
    const choices = [
      ...Object.values(draft.choices ?? {})
        .flat()
        .map((o) => `Escolha: ${optionLabel(o)}`),
      ...(draft.skillChoices ?? []).map(
        (k) => `Perícia de classe: ${SKILL_NAMES[k] ?? k} · Proficiência`,
      ),
      ...(draft.extraProficiencies ?? []).map(
        (k) => `Perícia do antecedente: ${SKILL_NAMES[k] ?? k} · Proficiência`,
      ),
      ...(draft.expertise ?? []).map(
        (k) => `Especialização: ${SKILL_NAMES[k] ?? k} · ${bonusType('expertise').label}`,
      ),
    ];
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
      { title: 'Escolhas', items: choices },
    ];
    return groups.filter((g) => g.items.length > 0);
  });

  constructor() {
    effect(() => {
      const id = this.store.draft().raceId;
      if (!id) {
        this.race.set(null);
      } else {
        void this.ruleset.race(id).then((r) => this.race.set(r));
      }
    });
  }
}

function bonusLine(b: SheetBonus): string {
  const target = b.target === 'hp-per-level' ? 'PV por nível' : optionLabel(b.target);
  return `${signed(b.value)} ${target} · ${bonusType(b.bonusType).label} · ${b.label ?? b.originName}`;
}

function traitLine(t: SheetTrait): string {
  const k = kind(t.kind);
  return `${t.name} · ${k.label}${t.kind === 'advantage' ? ' (role 2d20 e fique com o maior)' : ''}`;
}
