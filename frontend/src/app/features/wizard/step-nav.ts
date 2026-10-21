import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { neighbor } from './wizard-steps';

/**
 * Botões "Voltar"/"Próximo" do passo; `blockedReason` (do passo) ou um salvamento recusado/em
 * conflito impedem o avanço e explicam o motivo (F8 AC7).
 */
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
        @if (reason(); as reason) {
          <button type="button" disabled aria-describedby="blocked-reason">Próximo</button>
          <span id="blocked-reason" class="reason">{{ reason }}</span>
        } @else {
          <a [routerLink]="['..', n]" class="next button-primary">Próximo</a>
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
      margin-top: 2rem;
      padding-top: 1rem;
      border-top: 1px solid var(--border);
    }
    .reason {
      color: var(--danger);
    }
  `,
})
export class StepNavComponent {
  readonly step = input.required<string>();
  readonly blockedReason = input<string | null>(null);

  private readonly store = inject(CharacterDraftStore);

  protected readonly reason = computed(() => {
    const status = this.store.status();
    return (
      this.blockedReason() ??
      (status === 'invalid'
        ? 'Corrija os pontos listados no topo antes de continuar.'
        : status === 'conflict'
          ? 'Recarregue a ficha antes de continuar.'
          : null)
    );
  });
  protected readonly prev = computed(() => neighbor(this.step(), -1));
  protected readonly next = computed(() => neighbor(this.step(), 1));
}
