import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { saved } from '../../testing/fixtures';
import { polyfillDialog } from '../../testing/dialog-polyfill';
import { WizardShellComponent } from './wizard-shell';

polyfillDialog();

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

  it('erros do salvamento devem aparecer no topo com link para o passo de cada campo (F8 AC1)', async () => {
    const { fixture, el, store } = await render();

    store.errors.set({
      'choices.aumento-de-atributo': ['Escolha 2 atributos.'],
      level: ['O nível deve estar entre 1 e 20.'],
      'baseAbilities.forca': ['Força: use de 3 a 18.'],
      expertise: ['Especialização demais.'],
      name: ['Dê um nome à ficha.'],
    });
    store.status.set('invalid');
    await fixture.whenStable();

    const items = Array.from(el.querySelectorAll('.errors li'), (li) => ({
      text: li.textContent?.replace(/\s+/g, ' ').trim(),
      href: li.querySelector('a')?.getAttribute('href') ?? null,
    }));
    expect(items).toEqual([
      { text: 'Escolha 2 atributos. Ir para Raça', href: '/raca' },
      { text: 'O nível deve estar entre 1 e 20. Ir para Classe', href: '/classe' },
      { text: 'Força: use de 3 a 18. Ir para Atributos', href: '/atributos' },
      { text: 'Especialização demais. Ir para Perícias e salvaguardas', href: '/pericias' },
      { text: 'Dê um nome à ficha.', href: null },
    ]);
  });

  it('erro de rede deve mostrar o motivo, em cor de alerta, com "Tentar de novo" (F8 AC4)', async () => {
    const { fixture, el, store } = await render();
    const retry = vi.spyOn(store, 'retry').mockImplementation(() => undefined);

    store.failure.set('A API (servidor) não respondeu. Tente de novo em instantes.');
    store.status.set('error');
    await fixture.whenStable();
    (
      Array.from(el.querySelectorAll('button')).find(
        (b) => b.textContent?.trim() === 'Tentar de novo',
      ) as HTMLButtonElement
    ).click();

    expect([
      el.querySelector('[role=status]')?.textContent?.trim(),
      el.querySelector('[role=status]')?.classList.contains('problem'),
      retry.mock.calls.length,
    ]).toEqual(['Não salvo: A API (servidor) não respondeu. Tente de novo em instantes.', true, 1]);
  });

  it('renomear para vazio deve voltar ao nome anterior e avisar (F8 AC6)', async () => {
    const { fixture, el, store } = await render();
    const rename = vi.spyOn(store, 'rename');
    const input = el.querySelector('#sheet-name') as HTMLInputElement;

    input.value = '   ';
    input.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect([
      input.value,
      rename.mock.calls.length,
      el.querySelector(`#${input.getAttribute('aria-describedby')}`)?.textContent?.trim(),
    ]).toEqual(['Thorin', 0, 'O nome da ficha não pode ficar vazio.']);
  });

  it('abrir quando falha deve distinguir ficha inexistente, arquivo com problema e servidor fora (F8 AC8)', async () => {
    const texts = [];
    for (const error of [
      new HttpErrorResponse({ status: 404 }),
      new HttpErrorResponse({ status: 422, error: { detail: 'x.json: falta o campo "draft".' } }),
      new HttpErrorResponse({ status: 0 }),
    ]) {
      api.get.mockRejectedValueOnce(error);
      const { el } = await render();
      const alert = el.querySelector('[role=alert]');
      texts.push([
        alert?.querySelector('p')?.textContent?.trim(),
        Array.from(alert?.querySelectorAll('button') ?? [], (b) => b.textContent?.trim()),
      ]);
      TestBed.resetTestingModule();
    }

    expect(texts).toEqual([
      ['Ficha não encontrada: ela pode ter sido apagada.', []],
      ['O arquivo desta ficha está com problema: x.json: falta o campo "draft".', []],
      ['A API (servidor) não respondeu. Tente de novo em instantes.', ['Tentar de novo']],
    ]);
  });

  it('fechar a aba com salvamento pendente deve pedir confirmação; salvo, não (F8 AC12)', async () => {
    const { fixture, store } = await render();
    const leave = () => {
      const event = new Event('beforeunload', { cancelable: true });
      window.dispatchEvent(event);
      return event.defaultPrevented;
    };

    store.status.set('pending');
    await fixture.whenStable();
    const pending = leave();
    store.status.set('saved');
    await fixture.whenStable();

    expect([pending, leave()]).toEqual([true, false]);
  });

  it('conflito deve abrir diálogo modal que só fecha recarregando (F8 AC13)', async () => {
    const { fixture, el, store } = await render();
    const dialog = el.querySelector('dialog') as HTMLDialogElement;

    store.status.set('conflict');
    await fixture.whenStable();
    const cancel = new Event('cancel', { cancelable: true });
    dialog.dispatchEvent(cancel);

    expect([dialog.open, cancel.defaultPrevented]).toEqual([true, true]);
  });
});
