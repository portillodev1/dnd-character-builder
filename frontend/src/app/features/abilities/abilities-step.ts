import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-abilities-step',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Atributos</h2>
    <p>Em construção — ver .specs/project/spec.md (AD-011).</p>
  `,
})
export class AbilitiesStepComponent {}
