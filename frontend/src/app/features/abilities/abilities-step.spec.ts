import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
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
});
