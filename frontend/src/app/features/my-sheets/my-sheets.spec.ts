import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetSummary } from '../../core/api/models';
import { SheetService } from '../../core/api/sheet.service';
import { MySheetsComponent } from './my-sheets';

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
  };

  async function render() {
    api.list.mockResolvedValue([
      summary('1', 'Antiga', '2026-10-01T10:00:00Z'),
      summary('2', 'Recente', '2026-10-20T10:00:00Z'),
    ]);
    TestBed.configureTestingModule({
      imports: [MySheetsComponent],
      providers: [provideRouter([]), { provide: SheetService, useValue: api }],
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
});
