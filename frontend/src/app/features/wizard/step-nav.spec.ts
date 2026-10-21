import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SheetService } from '../../core/api/sheet.service';
import { SaveStatus, CharacterDraftStore } from '../../state/character-draft.store';
import { StepNavComponent } from './step-nav';

describe('StepNavComponent', () => {
  async function render(status: SaveStatus) {
    TestBed.configureTestingModule({
      imports: [StepNavComponent],
      providers: [provideRouter([]), { provide: SheetService, useValue: {} }],
    });
    TestBed.inject(CharacterDraftStore).status.set(status);
    const fixture = TestBed.createComponent(StepNavComponent);
    fixture.componentRef.setInput('step', 'classe');
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  it('"Próximo" quando o salvamento foi recusado ou conflitou deve ficar bloqueado com o motivo (F8 AC7)', async () => {
    const reasons = [];
    for (const status of ['invalid', 'conflict', 'saved'] as const) {
      const el = await render(status);
      reasons.push(el.querySelector('#blocked-reason')?.textContent?.trim() ?? null);
      TestBed.resetTestingModule();
    }

    expect(reasons).toEqual([
      'Corrija os pontos listados no topo antes de continuar.',
      'Recarregue a ficha antes de continuar.',
      null,
    ]);
  });
});
