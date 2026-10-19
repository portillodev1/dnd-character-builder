import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-sheet-step',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Ficha</h2>
    <p>Em construção — ver .specs/features/bonus-breakdown/spec.md.</p>
  `,
})
export class SheetStepComponent {}
