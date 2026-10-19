import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-race-step',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Raça</h2>
    <p>Em construção — ver .specs/features/race-traits/spec.md.</p>
  `,
})
export class RaceStepComponent {}
