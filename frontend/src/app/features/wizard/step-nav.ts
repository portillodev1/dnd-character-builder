import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { neighbor } from './wizard-steps';

/** Botões "Voltar"/"Próximo" do passo; `blockedReason` impede o avanço e explica o motivo. */
@Component({
  selector: 'app-step-nav',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="nav">
      @if (prev(); as p) {
        <a [routerLink]="['..', p]">Voltar</a>
      }
      @if (next(); as n) {
        @if (blockedReason(); as reason) {
          <button type="button" disabled aria-describedby="blocked-reason">Próximo</button>
          <span id="blocked-reason" class="reason">{{ reason }}</span>
        } @else {
          <a [routerLink]="['..', n]" class="next">Próximo</a>
        }
      }
    </div>
  `,
  styles: `
    .nav {
      display: flex;
      flex-wrap: wrap;
      gap: 1rem;
      align-items: center;
      margin-top: 1.5rem;
    }
    .reason {
      color: #a51d2d;
    }
  `,
})
export class StepNavComponent {
  readonly step = input.required<string>();
  readonly blockedReason = input<string | null>(null);

  protected readonly prev = computed(() => neighbor(this.step(), -1));
  protected readonly next = computed(() => neighbor(this.step(), 1));
}
