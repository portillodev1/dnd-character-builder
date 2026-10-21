import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { feature, sheet } from '../../testing/fixtures';
import { AbilitiesStepComponent } from './abilities-step';

describe('AbilitiesStepComponent', () => {
  async function render() {
    TestBed.configureTestingModule({
      imports: [AbilitiesStepComponent],
      providers: [provideRouter([]), { provide: SheetService, useValue: {} }],
    });
    const store = TestBed.inject(CharacterDraftStore);
    store.draft.set({ abilityMethod: 'manual', baseAbilities: { forca: 10 } });
    const fixture = TestBed.createComponent(AbilitiesStepComponent);
    await fixture.whenStable();
    return { fixture, store, el: fixture.nativeElement as HTMLElement };
  }

  it('valor base decimal deve avisar junto ao campo e não ser enviado (F8 AC3)', async () => {
    const { fixture, store, el } = await render();
    const input = el.querySelector('#base-forca') as HTMLInputElement;

    input.value = '12.5';
    input.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect([
      store.draft().baseAbilities?.['forca'],
      input.getAttribute('aria-invalid'),
      el.querySelector(`#${input.getAttribute('aria-describedby')}`)?.textContent?.trim(),
    ]).toEqual([10, 'true', 'Use um número inteiro.']);
  });

  it('compra de pontos deve mostrar o gasto vindo da API; acima do orçamento, só a mensagem da API (F9 AC1, AC2)', async () => {
    const { fixture, store, el } = await render();
    const budget = () => el.querySelector('.budget')?.textContent?.trim() ?? null;

    store.draft.set({ abilityMethod: 'point-buy', baseAbilities: { forca: 15 } });
    store.sheet.set(sheet({ pointBuy: { spent: 23, budget: 27 } }));
    await fixture.whenStable();
    const within = budget();
    store.errors.set({ baseAbilities: ['Compra de pontos: 54 pontos gastos, o máximo é 27.'] });
    await fixture.whenStable();

    expect([within, budget(), el.querySelector('.error')?.textContent?.trim()]).toEqual([
      'Pontos gastos: 23 de 27 (restam 4).',
      null,
      'Compra de pontos: 54 pontos gastos, o máximo é 27.',
    ]);
  });

  it('Incrementos da classe devem abrir dois +1 por nível e gravar a escolha no rascunho (F10 AC1)', async () => {
    const { fixture, store, el } = await render();
    const id = 'incremento-no-valor-de-habilidade-4';
    store.draft.set({ abilityMethod: 'manual', abilityImprovements: { [id]: ['destreza'] } });
    store.sheet.set(
      sheet({ features: [feature(), feature({ id: 'ataque', abilityImprovement: false })] }),
    );
    await fixture.whenStable();

    const legends = Array.from(el.querySelectorAll('.asi legend'), (l) => l.textContent?.trim());
    const second = el.querySelector(`#${id}-1`) as HTMLSelectElement;
    const firstValue = (el.querySelector(`#${id}-0`) as HTMLSelectElement).value;
    second.value = 'destreza';
    second.dispatchEvent(new Event('change'));

    expect([legends, firstValue, store.draft().abilityImprovements?.[id]]).toEqual([
      ['Incremento no Valor de Habilidade (nível 4) · LdJ p. 85'],
      'destreza',
      ['destreza', 'destreza'],
    ]);
  });
});
