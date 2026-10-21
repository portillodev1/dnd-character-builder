import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { SheetSummary } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import { SheetService } from '../../core/api/sheet.service';
import { polyfillDialog } from '../../testing/dialog-polyfill';
import { MySheetsComponent } from './my-sheets';

polyfillDialog();

const summary = (id: string, name: string, updatedAt: string): SheetSummary => ({
  id,
  name,
  raceId: 'anao',
  classId: 'ladino',
  level: 1,
  updatedAt,
  error: null,
});

describe('MySheetsComponent', () => {
  const api = {
    list: vi.fn(),
    delete: vi.fn().mockResolvedValue(undefined),
    create: vi.fn(),
    duplicate: vi.fn(),
  };
  const ruleset = {
    races: vi.fn().mockResolvedValue([{ id: 'anao', name: 'Anão' }]),
    classes: vi.fn().mockResolvedValue([{ id: 'ladino', name: 'Ladino' }]),
  };

  async function render() {
    api.list.mockResolvedValue([
      summary('1', 'Antiga', '2026-10-01T10:00:00Z'),
      summary('2', 'Recente', '2026-10-20T10:00:00Z'),
    ]);
    TestBed.configureTestingModule({
      imports: [MySheetsComponent],
      providers: [
        provideRouter([]),
        { provide: SheetService, useValue: api },
        { provide: RulesetService, useValue: ruleset },
      ],
    });
    const fixture = TestBed.createComponent(MySheetsComponent);
    await fixture.whenStable();
    return { fixture, el: fixture.nativeElement as HTMLElement };
  }

  const buttonByText = (el: HTMLElement, text: string) =>
    Array.from(el.querySelectorAll('button')).find(
      (b) => b.textContent?.trim() === text,
    ) as HTMLButtonElement;

  it('lista quando carregada deve mostrar a ficha mais recente primeiro (F5 AC1)', async () => {
    const { el } = await render();

    expect(Array.from(el.querySelectorAll('.list a'), (a) => a.textContent?.trim())).toEqual([
      'Recente',
      'Antiga',
    ]);
  });

  it('lista quando carregada deve mostrar nomes de raça e classe, não ids (F5 AC1)', async () => {
    const { el } = await render();

    expect(el.querySelector('.meta')?.textContent).toContain('Anão · Ladino · nível 1');
  });

  it('apagar quando confirmado deve remover o arquivo e a linha; antes disso pede confirmação (F5 AC7)', async () => {
    const { fixture, el } = await render();

    buttonByText(el, 'Apagar').click();
    await fixture.whenStable();
    expect(el.querySelector('[role=alertdialog]')?.textContent).toContain(
      'Apagar a ficha "Recente"?',
    );
    expect(api.delete).not.toHaveBeenCalled();

    buttonByText(el, 'Apagar ficha').click();
    await fixture.whenStable();

    expect(api.delete).toHaveBeenCalledWith('2');
    expect(Array.from(el.querySelectorAll('.list a'), (a) => a.textContent?.trim())).toEqual([
      'Antiga',
    ]);
  });

  it('duplicar deve criar a cópia pela API e abrir a cópia no passo Raça (F7 AC5)', async () => {
    const { fixture, el } = await render();
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    api.duplicate.mockResolvedValue({ id: '3' });

    buttonByText(el, 'Duplicar').click();
    await fixture.whenStable();

    expect([api.duplicate.mock.calls[0], navigate.mock.calls[0]]).toEqual([
      ['2'],
      [['/fichas', '3', 'raca']],
    ]);
  });

  it('duplicar ou apagar quando a API falha deve avisar em role=alert (F8 AC9)', async () => {
    const { fixture, el } = await render();
    const offline = new HttpErrorResponse({ status: 0 });
    api.duplicate.mockRejectedValueOnce(offline);
    api.delete.mockRejectedValueOnce(new HttpErrorResponse({ status: 404 }));
    const alerts = [];

    buttonByText(el, 'Duplicar').click();
    await fixture.whenStable();
    alerts.push(el.querySelector('[role=alert]')?.textContent?.trim());
    buttonByText(el, 'Apagar').click();
    await fixture.whenStable();
    buttonByText(el, 'Apagar ficha').click();
    await fixture.whenStable();
    alerts.push(el.querySelector('[role=alert]')?.textContent?.trim());

    expect(alerts).toEqual([
      'A API (servidor) não respondeu. Tente de novo em instantes.',
      'Ficha não encontrada: ela pode ter sido apagada.',
    ]);
  });

  it('ficha sem nível deve mostrar "nível a escolher" (F8 AC10)', async () => {
    api.list.mockResolvedValueOnce([
      { ...summary('1', 'Nova', '2026-10-20T10:00:00Z'), level: null },
    ]);
    TestBed.configureTestingModule({
      imports: [MySheetsComponent],
      providers: [
        provideRouter([]),
        { provide: SheetService, useValue: api },
        { provide: RulesetService, useValue: ruleset },
      ],
    });
    const fixture = TestBed.createComponent(MySheetsComponent);
    await fixture.whenStable();

    expect((fixture.nativeElement as HTMLElement).querySelector('.meta')?.textContent).toContain(
      'Anão · Ladino · nível a escolher',
    );
  });

  it('confirmação de apagar deve abrir como diálogo modal e "Cancelar"/Esc fecham sem apagar (F8 AC13)', async () => {
    const { fixture, el } = await render();
    const dialog = () => el.querySelector('dialog') as HTMLDialogElement;

    buttonByText(el, 'Apagar').click();
    await fixture.whenStable();
    const opened = dialog().open;
    dialog().close(); // Esc no navegador dispara cancel → close
    await fixture.whenStable();

    expect([
      opened,
      dialog().open,
      dialog().textContent?.trim(),
      api.delete.mock.calls.length,
    ]).toEqual([true, false, '', 0]);
  });
});
