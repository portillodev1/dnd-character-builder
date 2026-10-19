import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-class-step',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Classe e nível</h2>
    <p>Em construção — ver .specs/features/class-features/spec.md.</p>
  `,
})
export class ClassStepComponent {}
