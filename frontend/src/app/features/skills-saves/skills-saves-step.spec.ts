import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ClassFeatures } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { sheet, value } from '../../testing/fixtures';
import { SkillsSavesStepComponent } from './skills-saves-step';

const ladino = {
  name: 'Ladino',
  skillChoices: {
    count: 4,
    from: ['acrobacia', 'furtividade', 'percepcao', 'enganacao', 'intuicao'],
  },
} as ClassFeatures;

const skill = (sheetLabel: string, sources: string[] = []) => ({
  name: sheetLabel,
  sheetLabel,
  ability: 'sabedoria',
  proficiency: sources.length ? 'proficient' : 'none',
  proficiencySources: sources,
  ...value(sources.length ? 3 : 1),
});

describe('SkillsSavesStepComponent', () => {
  async function render() {
    TestBed.configureTestingModule({
      imports: [SkillsSavesStepComponent],
      providers: [
        provideRouter([]),
        { provide: SheetService, useValue: {} },
        { provide: RulesetService, useValue: { classFeatures: vi.fn().mockResolvedValue(ladino) } },
      ],
    });
    const store = TestBed.inject(CharacterDraftStore);
    store.draft.set({ classId: 'ladino', level: 1, skillChoices: ['furtividade', 'acrobacia'] });
    store.sheet.set(
      sheet({
        skills: {
          percepcao: skill('Percepção (Sab)', ['Elfo']),
          furtividade: skill('Furtividade (Des)', ['Ladino']),
          acrobacia: skill('Acrobacia (Des)', ['Ladino']),
        },
      }),
    );
    const fixture = TestBed.createComponent(SkillsSavesStepComponent);
    await fixture.whenStable();
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  it('contador quando 2 perícias escolhidas deve mostrar "2 de 4" (F3 AC2)', async () => {
    const el = await render();

    expect(el.querySelector('.counter')?.textContent).toContain('Perícias do Ladino: 2 de 4');
  });

  it('perícia racial deve ficar travada com a origem (F3 AC3)', async () => {
    const el = await render();

    const box = el.querySelector<HTMLInputElement>(
      'input[aria-label="Perícia de classe: Percepção"]',
    );
    expect(box?.disabled).toBe(true);
    expect(box?.title).toBe('Já proficiente por Elfo.');
  });
});
