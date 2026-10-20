import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { sheet } from '../../testing/fixtures';
import { SheetStepComponent } from './sheet-step';

const src = (page: number) => ({ book: 'LdJ', page });

describe('SheetStepComponent', () => {
  async function render() {
    TestBed.configureTestingModule({
      imports: [SheetStepComponent],
      providers: [provideRouter([]), { provide: SheetService, useValue: {} }],
    });
    const store = TestBed.inject(CharacterDraftStore);
    // O rascunho tem escolhas que a API não devolveu: o front não deve lê-las.
    store.draft.set({ raceId: 'meio-elfo', level: 1, skillChoices: ['acrobacia'] });
    store.sheet.set(
      sheet({
        speed: {
          total: 9,
          parts: [{ value: 9, label: 'Meio-Elfo', bonusType: 'speed', source: src(39) }],
        },
        bonuses: [
          {
            target: 'ability:carisma',
            value: 2,
            bonusType: 'ability-score-increase',
            label: null,
            origin: 'race',
            originName: 'Meio-Elfo',
            source: src(39),
          },
          {
            target: 'ability:forca',
            value: 1,
            bonusType: 'ability-score-increase',
            label: 'Aumento no Valor de Habilidade',
            origin: 'choice',
            originName: 'Meio-Elfo',
            source: src(39),
          },
          {
            target: 'skill:furtividade',
            value: 2,
            bonusType: 'expertise',
            label: 'Especialização',
            origin: 'choice',
            originName: 'Ladino',
            source: src(91),
          },
        ],
      }),
    );
    const fixture = TestBed.createComponent(SheetStepComponent);
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  const group = (el: HTMLElement, title: string) => {
    const h4 = Array.from(el.querySelectorAll('h4')).find((h) => h.textContent?.startsWith(title));
    return Array.from(h4?.nextElementSibling?.querySelectorAll('li') ?? [], (li) =>
      li.textContent?.trim(),
    );
  };

  it('"Meus bônus" › Escolhas deve listar os bônus que a API marcou como escolha (F4 AC4)', async () => {
    const el = await render();

    expect(group(el, 'Escolhas')).toEqual([
      '+1 Força · Aumento de atributo · Aumento no Valor de Habilidade (Meio-Elfo)',
      '+2 Furtividade · Especialização · Especialização (Ladino)',
    ]);
    expect(group(el, 'Raça')).toEqual(['+2 Carisma · Aumento de atributo · Meio-Elfo']);
  });

  it('deslocamento deve abrir a conta vinda da API (F4 AC1)', async () => {
    const el = await render();

    const button = el.querySelector<HTMLButtonElement>('button[aria-label^="Deslocamento"]');
    expect(button?.textContent?.trim()).toBe('9 m');
  });
});
