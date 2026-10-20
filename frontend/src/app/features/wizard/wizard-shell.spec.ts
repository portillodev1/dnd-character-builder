import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { saved } from '../../testing/fixtures';
import { WizardShellComponent } from './wizard-shell';

describe('WizardShellComponent', () => {
  const api = { get: vi.fn().mockResolvedValue(saved()) };

  async function render() {
    TestBed.configureTestingModule({
      imports: [WizardShellComponent],
      providers: [provideRouter([]), { provide: SheetService, useValue: api }],
    });
    const fixture = TestBed.createComponent(WizardShellComponent);
    fixture.componentRef.setInput('id', '00000000-0000-0000-0000-000000000001');
    await fixture.whenStable();
    await fixture.whenStable();
    return {
      fixture,
      el: fixture.nativeElement as HTMLElement,
      store: TestBed.inject(CharacterDraftStore),
    };
  }

  it('status quando salvo deve mostrar "Salvo às HH:MM" (F5 AC3)', async () => {
    const { fixture, el, store } = await render();

    store.status.set('saved');
    store.savedAt.set(new Date(2026, 9, 20, 18, 42));
    await fixture.whenStable();

    expect(el.querySelector('[role=status]')?.textContent?.trim()).toBe('Salvo às 18:42');
  });

  it('conflito quando outra aba salvou deve oferecer recarregar a ficha (F5 AC6)', async () => {
    const { fixture, el, store } = await render();

    store.status.set('conflict');
    await fixture.whenStable();
    (el.querySelector('[role=alertdialog] button') as HTMLButtonElement).click();
    await fixture.whenStable();

    expect(api.get).toHaveBeenCalledTimes(2);
    expect(store.status()).toBe('idle');
  });
});
