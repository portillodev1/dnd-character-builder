import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { Choice, Race, RaceBonus, RaceSummary } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import { bonusType, optionLabel, signed } from '../../shared/labels';
import { TraitCardComponent } from '../../shared/ui/trait-card';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { ChoicePickerComponent } from '../wizard/choice-picker';
import { StepNavComponent } from '../wizard/step-nav';

/** Passo 1 — raça e sub-raça com traços explicados (spec F1). */
@Component({
  selector: 'app-race-step',
  imports: [TraitCardComponent, ChoicePickerComponent, StepNavComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Raça</h2>
    <p class="help">
      A raça define traços do seu corpo e da sua cultura: aumentos de atributo, sentidos, idiomas e
      talentos naturais (LdJ cap. 2, p. 17).
    </p>

    <fieldset class="races">
      <legend>Escolha a raça</legend>
      @for (r of races(); track r.id) {
        <label class="race">
          <input
            type="radio"
            name="race"
            [value]="r.id"
            [checked]="r.id === raceId()"
            (change)="pickRace(r.id)"
          />
          <strong>{{ r.name }}</strong>
          <span>{{ r.summary }}</span>
          <span class="bonuses">{{ increases(r) }}</span>
        </label>
      }
    </fieldset>

    @if (notice(); as n) {
      <p role="status" class="notice">{{ n }}</p>
    }

    @if (race(); as r) {
      @if (r.subraces.length > 0) {
        <fieldset>
          <legend>Escolha a sub-raça de {{ r.name }}</legend>
          @for (s of r.subraces; track s.id) {
            <label class="race">
              <input
                type="radio"
                name="subrace"
                [value]="s.id"
                [checked]="s.id === subraceId()"
                (change)="pickSubrace(s.id)"
              />
              <strong>{{ s.name }}</strong> <span>{{ s.summary }}</span>
            </label>
          }
        </fieldset>
      }

      <section aria-labelledby="race-basics">
        <h3 id="race-basics">Básico</h3>
        <ul>
          <li>Tamanho: {{ r.size === 'pequeno' ? 'Pequeno' : 'Médio' }}</li>
          @if (speed(); as sp) {
            <li>
              Deslocamento: {{ meters(sp.meters) }} m (LdJ p. {{ sp.source.page }})
              @if (sp.note) {
                — {{ sp.note }}
              }
            </li>
          }
          <li>Idiomas: {{ r.languages.join(', ') }}</li>
          @for (b of allBonuses(); track $index) {
            <li>{{ bonusText(b) }} (LdJ p. {{ b.source.page }})</li>
          }
        </ul>
      </section>

      @for (c of choices(); track c.id) {
        <app-choice-picker
          [choice]="c"
          [picked]="picked(c.id)"
          (toggled)="toggleChoice(c, $event)"
        />
      }

      @for (group of traitGroups(); track group.origin) {
        <section class="traits" [attr.aria-label]="'Traços de ' + group.origin">
          <h3>{{ group.origin }}</h3>
          @for (t of group.traits; track t.id) {
            <app-trait-card
              [name]="t.name"
              [kind]="t.kind"
              [description]="t.description"
              [page]="t.source.page"
            />
          }
        </section>
      }
    }

    <app-step-nav step="raca" [blockedReason]="blockedReason()" />
  `,
  styles: `
    .races,
    fieldset {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(15rem, 1fr));
      gap: 0.75rem;
    }
    .race {
      display: grid;
      grid-template-columns: auto 1fr;
      align-content: start;
      gap: 0.2rem 0.6rem;
      padding: 0.75rem;
      border: var(--rule);
      border-radius: var(--radius);
      background: transparent;
      cursor: pointer;
    }
    .race:hover {
      border-color: var(--control-border);
    }
    .race:has(:checked) {
      border-color: var(--accent);
      background: var(--accent-soft);
    }
    .race input {
      align-self: center;
    }
    .race strong {
      font-family: var(--font-display);
      font-size: 1.35rem;
      font-weight: 400;
      color: var(--accent);
    }
    .race span {
      grid-column: 2;
      font-size: 0.92em;
    }
    .bonuses {
      color: var(--muted);
      font-style: italic;
    }
    section ul {
      margin: 0;
      padding: 0.75rem 1rem 0.75rem 2rem;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .traits {
      display: grid;
      gap: 0.75rem;
    }
    .traits h3 {
      margin-bottom: 0;
    }
    .notice {
      padding: 0.5rem 0.75rem;
      border-left: 3px double var(--warning);
      border-radius: var(--radius-sm);
      background: var(--warning-soft);
      color: var(--warning);
    }
  `,
})
export class RaceStepComponent {
  private readonly ruleset = inject(RulesetService);
  protected readonly store = inject(CharacterDraftStore);

  protected readonly races = signal<RaceSummary[]>([]);
  protected readonly race = signal<Race | null>(null);
  protected readonly notice = signal<string | null>(null);

  protected readonly raceId = computed(() => this.store.draft().raceId ?? null);
  protected readonly subraceId = computed(() => this.store.draft().subraceId ?? null);
  protected readonly subrace = computed(
    () => this.race()?.subraces.find((s) => s.id === this.subraceId()) ?? null,
  );
  /** A sub-raça pode trocar o deslocamento da raça (ex.: Elfo da Floresta, LdJ p. 24). */
  protected readonly speed = computed(() => this.subrace()?.speed ?? this.race()?.speed ?? null);
  protected readonly allBonuses = computed(() => [
    ...(this.race()?.bonuses ?? []),
    ...(this.subrace()?.bonuses ?? []),
  ]);
  protected readonly choices = computed<Choice[]>(() => [
    ...(this.race()?.choices ?? []),
    ...(this.subrace()?.choices ?? []),
  ]);
  /** Traços agrupados por origem: raça-mãe e sub-raça separadas (F1 AC3). */
  protected readonly traitGroups = computed(() => {
    const r = this.race();
    if (!r) {
      return [];
    }
    const s = this.subrace();
    return [
      { origin: r.name, traits: r.traits },
      ...(s ? [{ origin: s.name, traits: s.traits }] : []),
    ];
  });
  protected readonly blockedReason = computed(() => {
    const r = this.race();
    if (!r) {
      return 'Escolha uma raça para continuar.';
    }
    if (r.subraces.length > 0 && !this.subrace()) {
      return `Escolha uma sub-raça de ${r.name}.`;
    }
    const pending = this.choices().find((c) => this.picked(c.id).length < c.count);
    return pending ? `${pending.name}: faltam escolhas.` : null;
  });

  constructor() {
    void this.ruleset.races().then((list) => this.races.set(list));
    effect(() => {
      const id = this.raceId();
      if (id === null) {
        this.race.set(null);
      } else if (this.race()?.id !== id) {
        void this.ruleset.race(id).then((r) => this.race.set(r));
      }
    });
  }

  protected picked(choiceId: string): string[] {
    return this.store.draft().choices?.[choiceId] ?? [];
  }

  protected pickRace(id: string): void {
    const previous = this.raceId();
    if (previous === id) {
      return;
    }
    // Trocar de raça limpa sub-raça e escolhas da raça anterior (F1 AC7).
    this.store.update({ raceId: id, subraceId: null, choices: {} });
    this.notice.set(
      previous
        ? 'Você trocou de raça: a sub-raça e as escolhas da raça anterior foram limpas.'
        : null,
    );
  }

  protected pickSubrace(id: string): void {
    const keep = new Set((this.race()?.choices ?? []).map((c) => c.id));
    const choices = Object.fromEntries(
      Object.entries(this.store.draft().choices ?? {}).filter(([k]) => keep.has(k)),
    );
    this.store.update({ subraceId: id, choices });
  }

  protected toggleChoice(choice: Choice, option: string): void {
    this.store.update((d) => {
      const current = d.choices?.[choice.id] ?? [];
      const picks = current.includes(option)
        ? current.filter((o) => o !== option)
        : current.length < choice.count
          ? [...current, option]
          : current;
      return { ...d, choices: { ...(d.choices ?? {}), [choice.id]: picks } };
    });
  }

  protected meters(value: number): string {
    return value.toLocaleString('pt-BR');
  }

  protected increases(r: RaceSummary): string {
    return r.abilityIncreases.map((b) => `${signed(b.value)} ${optionLabel(b.target)}`).join(', ');
  }

  protected bonusText(b: RaceBonus): string {
    const target = b.target === 'hp-per-level' ? 'pontos de vida por nível' : optionLabel(b.target);
    return `${signed(b.value)} ${target} · ${bonusType(b.bonusType).label} · ${b.label ?? ''}`;
  }
}
