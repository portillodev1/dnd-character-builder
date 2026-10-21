import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { ClassFeatures, ClassSummary } from '../../core/api/models';
import { latestOnly } from '../../core/api/latest-only';
import { RulesetService } from '../../core/api/ruleset.service';
import { ABILITY_NAMES, SKILL_NAMES } from '../../shared/labels';
import { TraitCardComponent } from '../../shared/ui/trait-card';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { StepNavComponent } from '../wizard/step-nav';

const LEVELS = Array.from({ length: 20 }, (_, i) => i + 1);

/** Passo 2 — classe e nível com as características até o nível (spec F2). */
@Component({
  selector: 'app-class-step',
  imports: [TraitCardComponent, StepNavComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Classe e nível</h2>
    <p class="help">
      A classe é a profissão do aventureiro: define pontos de vida, proficiências, salvaguardas e o
      que você sabe fazer em cada nível (LdJ cap. 3, p. 45).
    </p>

    <fieldset class="classes">
      <legend>Escolha a classe</legend>
      @for (c of classes(); track c.id) {
        <label class="class">
          <input
            type="radio"
            name="class"
            [value]="c.id"
            [checked]="c.id === classId()"
            (change)="pickClass(c.id)"
          />
          <strong>{{ c.name }}</strong>
          <span>{{ c.summary }}</span>
          <span class="meta">
            Dado de Vida d{{ c.hitDie }} · Atributo principal: {{ names(c.primaryAbilities) }} ·
            Salvaguardas: {{ names(c.savingThrows) }}
          </span>
        </label>
      }
    </fieldset>

    <label for="level">Nível do personagem</label>
    <select id="level" [value]="level()" (change)="pickLevel($event)">
      @for (n of levels; track n) {
        <option [value]="n" [selected]="n === level()">{{ n }}</option>
      }
    </select>

    @if (notice(); as n) {
      <p role="status" class="notice">{{ n }}</p>
    }

    @if (details(); as d) {
      <section aria-labelledby="profs">
        <h3 id="profs">Proficiências de {{ d.name }}</h3>
        <ul>
          <li>Armaduras: {{ d.proficiencies.armor.join(', ') || 'nenhuma' }}</li>
          <li>Armas: {{ d.proficiencies.weapons.join(', ') || 'nenhuma' }}</li>
          <li>Ferramentas: {{ d.proficiencies.tools.join(', ') || 'nenhuma' }}</li>
          <li>Salvaguardas: {{ names(d.savingThrows) }}</li>
          <li>
            Perícias: escolha {{ d.skillChoices.count }} entre {{ skills(d.skillChoices.from) }}
          </li>
          <li>Pontos de vida: Dado de Vida d{{ d.hitDie }} por nível</li>
        </ul>
      </section>

      <section aria-labelledby="features" class="features">
        <h3 id="features">Características até o nível {{ d.level }}</h3>
        @for (f of d.features; track f.id) {
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

      @if (d.nextLevel.length > 0) {
        <section aria-labelledby="next-level" class="features">
          <h3 id="next-level">Próximo nível ({{ d.level + 1 }})</h3>
          @for (f of d.nextLevel; track f.id) {
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
      }
    }

    <app-step-nav
      step="classe"
      [blockedReason]="classId() ? null : 'Escolha uma classe para continuar.'"
    />
  `,
  styles: `
    .notice {
      padding: 0.5rem 0.75rem;
      border-left: 3px double var(--warning);
      border-radius: var(--radius-sm);
      background: var(--warning-soft);
    }
    .classes {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(15rem, 1fr));
      gap: 0.75rem;
    }
    .class {
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
    .class:hover {
      border-color: var(--control-border);
    }
    .class:has(:checked) {
      border-color: var(--accent);
      background: var(--accent-soft);
    }
    .class input {
      align-self: center;
    }
    .class strong {
      font-family: var(--font-display);
      font-size: 1.35rem;
      font-weight: 400;
      color: var(--accent);
    }
    .class span {
      grid-column: 2;
      font-size: 0.92em;
    }
    .meta {
      color: var(--muted);
      font-size: 0.85em;
    }
    label[for='level'] {
      margin-right: 0.5rem;
      font-weight: 600;
    }
    section ul {
      margin: 0;
      padding: 0.75rem 1rem 0.75rem 2rem;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .features {
      display: grid;
      gap: 0.75rem;
    }
    .features h3 {
      margin-bottom: 0;
    }
  `,
})
export class ClassStepComponent {
  private readonly ruleset = inject(RulesetService);
  private readonly store = inject(CharacterDraftStore);

  protected readonly levels = LEVELS;
  protected readonly classes = signal<ClassSummary[]>([]);
  protected readonly details = signal<ClassFeatures | null>(null);
  private readonly showDetails = latestOnly<ClassFeatures | null>((d) => this.details.set(d));
  protected readonly notice = signal<string | null>(null);
  protected readonly classId = computed(() => this.store.draft().classId ?? null);
  protected readonly level = computed(() => this.store.draft().level ?? 1);

  constructor() {
    void this.ruleset.classes().then((list) => this.classes.set(list));
    effect(() => {
      const id = this.classId();
      const level = this.level();
      this.showDetails(id === null ? Promise.resolve(null) : this.ruleset.classFeatures(id, level));
    });
  }

  protected pickClass(id: string): void {
    if (id === this.classId()) {
      return;
    }
    // Perícias e especializações dependem da classe: trocar de classe as limpa.
    this.store.update({ classId: id, level: this.level(), skillChoices: [], expertise: [] });
  }

  /**
   * Muda o nível; se a classe passa a conceder menos Especializações (contagem da API), desmarca
   * as excedentes na mesma mudança, para o salvamento não ser recusado (F8 AC2).
   */
  protected async pickLevel(event: Event): Promise<void> {
    const level = Number((event.target as HTMLSelectElement).value);
    const id = this.classId();
    const expertise = this.store.draft().expertise ?? [];
    const allowed =
      id === null || expertise.length === 0
        ? null
        : await this.ruleset.classFeatures(id, level).catch(() => null);
    const removed = allowed ? expertise.slice(allowed.expertiseCount) : [];
    this.store.update(
      removed.length > 0
        ? { level, expertise: expertise.slice(0, allowed!.expertiseCount) }
        : { level },
    );
    this.notice.set(
      removed.length > 0
        ? `No nível ${level} o ${allowed!.name} concede ${allowed!.expertiseCount} Especializações: ` +
            `desmarcamos ${this.skillList(removed)}.`
        : null,
    );
  }

  protected names(ids: readonly string[]): string {
    return ids.map((a) => ABILITY_NAMES[a] ?? a).join(', ');
  }

  protected skills(ids: readonly string[]): string {
    return ids.map((s) => SKILL_NAMES[s] ?? s).join(', ');
  }

  /** "Acrobacia e Enganação" / "Acrobacia, Enganação e Furtividade". */
  private skillList(ids: readonly string[]): string {
    const names = ids.map((s) => SKILL_NAMES[s] ?? s);
    return names.length > 1 ? `${names.slice(0, -1).join(', ')} e ${names.at(-1)}` : names[0];
  }
}
