import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RulesetService } from '../../core/api/ruleset.service';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { anao, meioElfo } from '../../testing/fixtures';
import { RaceStepComponent } from './race-step';

describe('RaceStepComponent', () => {
  async function render(draft: object, anaoRace = anao) {
    TestBed.configureTestingModule({
      imports: [RaceStepComponent],
      providers: [
        provideRouter([]),
        { provide: SheetService, useValue: {} },
        {
          provide: RulesetService,
          useValue: {
            races: vi.fn().mockResolvedValue([]),
            race: vi.fn((id: string) => Promise.resolve(id === 'anao' ? anaoRace : meioElfo)),
          },
        },
      ],
    });
    TestBed.inject(CharacterDraftStore).draft.set(draft);
    const fixture = TestBed.createComponent(RaceStepComponent);
    await fixture.whenStable();
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  it('traços quando Anão da Colina deve agrupar por raça e sub-raça (F1 AC3)', async () => {
    const el = await render({ raceId: 'anao', subraceId: 'anao-da-colina' });

    const groups = Array.from(el.querySelectorAll('section.traits'), (s) => ({
      origin: s.querySelector('h3')?.textContent?.trim(),
      traits: Array.from(s.querySelectorAll('h4'), (h) => h.textContent?.trim()),
    }));
    expect(groups).toEqual([
      { origin: 'Anão', traits: ['Visão no Escuro'] },
      { origin: 'Anão da Colina', traits: ['Tenacidade Anã'] },
    ]);
  });

  it('deslocamento quando a sub-raça o altera deve mostrar o da sub-raça', async () => {
    const colina = { ...anao.subraces[0], speed: { ...anao.speed, meters: 10.5 } };
    const el = await render(
      { raceId: 'anao', subraceId: colina.id },
      { ...anao, subraces: [colina] },
    );

    expect(el.querySelector('#race-basics + ul')?.textContent).toContain('Deslocamento: 10,5 m');
  });

  it('próximo quando há escolha pendente deve ficar bloqueado com o motivo (F1 AC6)', async () => {
    const el = await render({
      raceId: 'meio-elfo',
      choices: { 'aumento-de-atributo': ['ability:forca'] },
    });

    const next = Array.from(el.querySelectorAll('app-step-nav button')).find(
      (b) => b.textContent?.trim() === 'Próximo',
    );
    expect(next?.hasAttribute('disabled')).toBe(true);
    expect(el.querySelector('#blocked-reason')?.textContent).toBe(
      'Aumento de Atributo (Meio-Elfo): faltam escolhas.',
    );
  });
});
