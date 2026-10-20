import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { ClassFeatures } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import { ABILITY_NAMES, SKILL_NAMES, signed } from '../../shared/labels';
import { BonusBreakdownComponent } from '../../shared/ui/bonus-breakdown';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { StepNavComponent } from '../wizard/step-nav';

const BACKGROUND = 'Antecedente';
/** AD-012: no MVP o antecedente concede 2 perícias marcadas à mão. */
const BACKGROUND_SKILLS = 2;
const SKILLS = Object.keys(SKILL_NAMES);
const PROFICIENCY_TEXT: Readonly<Record<string, string>> = {
  none: 'sem proficiência',
  half: 'Versatilidade (½)',
  proficient: 'proficiente',
  expertise: 'Especialização (×2)',
};

/** Passo 4 — escolher perícias e ver salvaguardas, perícias e passiva com a conta (spec F3). */
@Component({
  selector: 'app-skills-saves-step',
  imports: [BonusBreakdownComponent, StepNavComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Perícias e salvaguardas</h2>
    <p class="help">
      Perícia = modificador do atributo-chave + bônus de proficiência se você for treinado (LdJ p.
      176). Salvaguarda segue a mesma conta com o atributo (LdJ p. 181).
    </p>

    @for (e of fieldErrors(); track $index) {
      <p role="alert" class="error">{{ e }}</p>
    }

    @if (!store.sheet()) {
      <p>Corrija os erros dos passos anteriores para ver as perícias.</p>
    } @else {
      @if (details(); as d) {
        <p class="counter" aria-live="polite">
          Perícias do {{ d.name }}: {{ classPicks().length }} de {{ d.skillChoices.count }} ·
          Antecedente: {{ backgroundPicks().length }} de {{ backgroundCount }}
          @if (d.expertiseCount > 0) {
            · Especialização: {{ expertisePicks().length }} de {{ d.expertiseCount }}
          }
        </p>
      } @else {
        <p>Escolha a classe no passo anterior para marcar as perícias da classe.</p>
      }

      <div class="table-scroll" role="region" aria-label="Tabela de perícias" tabindex="0">
        <table class="skills">
          <caption>
            Perícias
          </caption>
          <thead>
            <tr>
              <th scope="col">Perícia</th>
              <th scope="col">Classe</th>
              <th scope="col">Antecedente</th>
              <th scope="col">Especialização</th>
              <th scope="col">Proficiência</th>
              <th scope="col">Valor</th>
            </tr>
          </thead>
          <tbody>
            @for (row of rows(); track row.id) {
              <tr>
                <th scope="row">{{ row.sheetLabel }}</th>
                <td>
                  <input
                    type="checkbox"
                    [attr.aria-label]="'Perícia de classe: ' + row.name"
                    [checked]="row.classPicked"
                    [disabled]="!!row.classBlocked"
                    [title]="row.classBlocked ?? ''"
                    (change)="toggleClass(row.id)"
                  />
                  @if (row.classBlocked && !row.classPicked) {
                    <span class="reason">{{ row.classBlocked }}</span>
                  }
                </td>
                <td>
                  <input
                    type="checkbox"
                    [attr.aria-label]="'Perícia do antecedente: ' + row.name"
                    [checked]="row.backgroundPicked"
                    [disabled]="!!row.backgroundBlocked"
                    [title]="row.backgroundBlocked ?? ''"
                    (change)="toggleBackground(row.id)"
                  />
                </td>
                <td>
                  @if (row.proficient) {
                    <input
                      type="checkbox"
                      [attr.aria-label]="'Especialização: ' + row.name"
                      [checked]="row.expertise"
                      [disabled]="!!row.expertiseBlocked"
                      [title]="row.expertiseBlocked ?? ''"
                      (change)="toggleExpertise(row.id)"
                    />
                  }
                </td>
                <td>
                  <span class="dot" [class.on]="row.proficient" aria-hidden="true"></span>
                  {{ row.proficiencyText }}
                  @if (row.sources.length > 0) {
                    <small>({{ row.sources.join(', ') }})</small>
                  }
                </td>
                <td>
                  @if (row.total !== null && row.parts) {
                    <app-bonus-breakdown
                      [label]="row.name"
                      [total]="row.total"
                      [parts]="row.parts"
                      [withSign]="true"
                    />
                  } @else {
                    —
                  }
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      <table class="saves">
        <caption>
          Salvaguardas
        </caption>
        <tbody>
          @for (s of saves(); track s.id) {
            <tr>
              <th scope="row">{{ s.name }}</th>
              <td>
                <span class="dot" [class.on]="s.proficient" aria-hidden="true"></span
                >{{ s.proficient ? 'proficiente' : '' }}
              </td>
              <td>
                <app-bonus-breakdown
                  [label]="'Salvaguarda de ' + s.name"
                  [total]="s.total"
                  [parts]="s.parts"
                  [withSign]="true"
                />
              </td>
            </tr>
          }
        </tbody>
      </table>

      @if (store.sheet()?.passivePerception; as p) {
        <p>
          Sabedoria Passiva (Percepção):
          <app-bonus-breakdown
            label="Sabedoria Passiva (Percepção)"
            [total]="p.total"
            [parts]="p.parts"
          />
        </p>
      }

      <section aria-labelledby="paper">
        <h3 id="paper">Como preencher na ficha de papel</h3>
        <ul>
          @for (line of paperLines(); track $index) {
            <li>{{ line }}</li>
          }
        </ul>
      </section>
    }

    <app-step-nav step="pericias" />
  `,
  styles: `
    table {
      margin: 1.5rem 0;
    }
    th,
    td {
      padding: 0.3rem 0.6rem;
    }
    tbody th {
      font-weight: 600;
    }
    tbody tr:hover > * {
      background: var(--surface-2);
    }
    .dot {
      display: inline-block;
      width: 0.8rem;
      height: 0.8rem;
      margin-right: 0.25rem;
      border: 2px solid var(--ink);
      border-radius: 50%;
      vertical-align: middle;
    }
    .dot.on {
      background: var(--accent);
      border-color: var(--accent);
    }
    .reason {
      display: block;
      font-size: 0.8em;
      color: var(--muted);
    }
    .table-scroll {
      margin: 1.5rem 0;
      overflow-x: auto;
    }
    .table-scroll table {
      margin: 0;
      min-width: 36rem;
    }
    .error {
      color: var(--danger);
    }
    .counter {
      display: inline-block;
      padding: 0.35rem 0.75rem;
      border-radius: var(--radius-sm);
      border: 1px solid var(--border);
      font-style: italic;
    }
    section ul {
      padding: 0.75rem 1rem 0.75rem 2rem;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: var(--surface);
    }
  `,
})
export class SkillsSavesStepComponent {
  private readonly ruleset = inject(RulesetService);
  protected readonly store = inject(CharacterDraftStore);

  protected readonly backgroundCount = BACKGROUND_SKILLS;
  protected readonly details = signal<ClassFeatures | null>(null);
  protected readonly classPicks = computed(() => this.store.draft().skillChoices ?? []);
  protected readonly backgroundPicks = computed(() => this.store.draft().extraProficiencies ?? []);
  protected readonly expertisePicks = computed(() => this.store.draft().expertise ?? []);

  protected readonly fieldErrors = computed(() =>
    Object.entries(this.store.errors())
      .filter(([k]) => ['skillChoices', 'extraProficiencies', 'expertise'].includes(k))
      .flatMap(([, v]) => v),
  );

  protected readonly rows = computed(() => {
    const sheet = this.store.sheet();
    const d = this.details();
    const className = d?.name;
    const classFull = d !== null && this.classPicks().length >= d.skillChoices.count;
    const backgroundFull = this.backgroundPicks().length >= BACKGROUND_SKILLS;
    // Quantas Especializações a classe concede no nível vem da API (F3 AC5).
    const expertiseCount = d?.expertiseCount ?? 0;
    const expertiseFull = this.expertisePicks().length >= expertiseCount;
    return SKILLS.map((id) => {
      const skill = sheet?.skills[id];
      const sources = skill?.proficiencySources ?? [];
      const classPicked = this.classPicks().includes(id);
      const backgroundPicked = this.backgroundPicks().includes(id);
      // Proficiência que não veio de classe nem antecedente (ex.: Elfo → Percepção) fica travada.
      const lockedBy = sources.filter((s) => s !== className && s !== BACKGROUND);
      const otherThanClass = lockedBy.length > 0 ? lockedBy : backgroundPicked ? [BACKGROUND] : [];
      const otherThanBackground =
        lockedBy.length > 0 ? lockedBy : classPicked && className ? [className] : [];
      const classBlocked = classPicked
        ? null
        : !d
          ? 'Escolha a classe antes.'
          : !d.skillChoices.from.includes(id)
            ? `Fora da lista do ${d.name}.`
            : otherThanClass.length > 0
              ? `Já proficiente por ${otherThanClass.join(', ')}.`
              : classFull
                ? `Limite de ${d.skillChoices.count} atingido.`
                : null;
      const backgroundBlocked = backgroundPicked
        ? null
        : otherThanBackground.length > 0
          ? `Já proficiente por ${otherThanBackground.join(', ')}.`
          : backgroundFull
            ? `Limite de ${BACKGROUND_SKILLS} atingido.`
            : null;
      const expertise = this.expertisePicks().includes(id);
      const expertiseBlocked = expertise
        ? null
        : expertiseCount === 0
          ? 'Sua classe e nível não concedem Especialização.'
          : expertiseFull
            ? `Limite de ${expertiseCount} atingido.`
            : null;
      const proficiency = skill?.proficiency ?? 'none';
      return {
        id,
        name: SKILL_NAMES[id] ?? id,
        sheetLabel: skill?.sheetLabel ?? SKILL_NAMES[id] ?? id,
        classPicked,
        backgroundPicked,
        classBlocked,
        backgroundBlocked,
        sources,
        proficient: proficiency === 'proficient' || proficiency === 'expertise',
        expertise,
        expertiseBlocked,
        proficiencyText: PROFICIENCY_TEXT[proficiency] ?? proficiency,
        total: skill?.total ?? null,
        parts: skill?.parts ?? null,
      };
    });
  });

  protected readonly saves = computed(() =>
    Object.entries(this.store.sheet()?.savingThrows ?? {}).map(([id, s]) => ({
      id,
      name: ABILITY_NAMES[id] ?? id,
      ...s,
    })),
  );

  /** Linhas da ficha de papel, montadas só com o que a API devolveu (F3 AC8). */
  protected readonly paperLines = computed(() => {
    const sheet = this.store.sheet();
    if (!sheet) {
      return [];
    }
    const lines: string[] = [];
    if (sheet.proficiencyBonus) {
      lines.push(`Bônus de Proficiência: escreva ${signed(sheet.proficiencyBonus.total)}.`);
    }
    for (const s of this.saves()) {
      lines.push(
        `Salvaguarda de ${s.name}: escreva ${signed(s.total)}${s.proficient ? ' e pinte a bolinha' : ''}.`,
      );
    }
    for (const r of this.rows()) {
      if (r.total !== null) {
        lines.push(
          `${r.sheetLabel}: escreva ${signed(r.total)}${r.proficient ? ' e pinte a bolinha' : ''}.`,
        );
      }
    }
    if (sheet.passivePerception) {
      lines.push(`Sabedoria Passiva (Percepção): escreva ${sheet.passivePerception.total}.`);
    }
    return lines;
  });

  constructor() {
    effect(() => {
      const id = this.store.draft().classId;
      const level = this.store.draft().level ?? 1;
      if (!id) {
        this.details.set(null);
      } else {
        void this.ruleset.classFeatures(id, level).then((d) => this.details.set(d));
      }
    });
  }

  protected toggleClass(id: string): void {
    this.store.update((d) => ({
      ...d,
      skillChoices: toggle(d.skillChoices, id),
      expertise: expertiseAfter(d.expertise, d.skillChoices, id),
    }));
  }

  protected toggleBackground(id: string): void {
    this.store.update((d) => ({
      ...d,
      extraProficiencies: toggle(d.extraProficiencies, id),
      expertise: expertiseAfter(d.expertise, d.extraProficiencies, id),
    }));
  }

  protected toggleExpertise(id: string): void {
    this.store.update((d) => ({ ...d, expertise: toggle(d.expertise, id) }));
  }
}

function toggle(list: readonly string[] | null | undefined, id: string): string[] {
  const current = list ?? [];
  return current.includes(id) ? current.filter((x) => x !== id) : [...current, id];
}

/** Desmarcar a proficiência também desmarca a Especialização dela (exige proficiência). */
function expertiseAfter(
  expertise: readonly string[] | null | undefined,
  picks: readonly string[] | null | undefined,
  id: string,
): string[] {
  const removing = (picks ?? []).includes(id);
  return (expertise ?? []).filter((e) => !removing || e !== id);
}
