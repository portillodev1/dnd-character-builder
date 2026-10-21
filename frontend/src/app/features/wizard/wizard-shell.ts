import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  afterRenderEffect,
  viewChild,
  effect,
  inject,
  input,
  signal,
  untracked,
  computed,
} from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { apiFailure } from '../../core/api/api-failure';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { stepOfField, wizardSteps } from './wizard-steps';

/** Ficha aberta: nome, passos, salvamento automático, conflito de versão e erros por passo (F5 AC3–AC6, F8). */
@Component({
  selector: 'app-wizard-shell',
  imports: [DatePipe, RouterLink, RouterLinkActive, RouterOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '(window:beforeunload)': 'confirmLeave($event)' },
  template: `
    @if (loadError(); as error) {
      <div role="alert" class="errors">
        <p>{{ error.message }}</p>
        @if (error.canRetry) {
          <button type="button" (click)="reload()">Tentar de novo</button>
        }
        <a routerLink="/fichas">Voltar para Minhas fichas</a>
      </div>
    } @else if (store.id()) {
      <div class="bar">
        <label for="sheet-name">Nome</label>
        <input
          id="sheet-name"
          [value]="store.name()"
          (change)="rename($event)"
          maxlength="80"
          [attr.aria-invalid]="nameError() ? 'true' : null"
          [attr.aria-describedby]="nameError() ? 'sheet-name-error' : null"
        />
        @if (nameError(); as error) {
          <span id="sheet-name-error" class="error" role="alert">{{ error }}</span>
        }
        <span class="status" [class.problem]="problem()" role="status" aria-live="polite">
          @switch (store.status()) {
            @case ('pending') {
              Alterações não salvas…
            }
            @case ('saving') {
              Salvando…
            }
            @case ('saved') {
              Salvo às {{ store.savedAt() | date: 'HH:mm' }}
            }
            @case ('invalid') {
              Não salvo: corrija os pontos listados abaixo.
            }
            @case ('error') {
              Não salvo: {{ store.failure() }}
            }
          }
        </span>
        @if (store.status() === 'error') {
          <button type="button" (click)="store.retry()">Tentar de novo</button>
        }
        <a routerLink="/fichas">Minhas fichas</a>
      </div>

      @if (errorList().length > 0) {
        <div class="errors" role="alert">
          <p>Não salvo: corrija estes pontos.</p>
          <ul>
            @for (e of errorList(); track $index) {
              <li>
                {{ e.message }}
                @if (e.step; as step) {
                  <a [routerLink]="step.path">Ir para {{ step.label }}</a>
                }
              </li>
            }
          </ul>
        </div>
      }
      <!-- Modal nativo que só fecha recarregando: Esc não descarta o aviso (F5 AC6, F8 AC13). -->
      <dialog
        #conflictDialog
        role="alertdialog"
        aria-labelledby="conflict-title"
        class="conflict"
        (cancel)="$event.preventDefault()"
      >
        @if (store.status() === 'conflict') {
          <p id="conflict-title">
            Esta ficha foi alterada em outra aba. Suas últimas mudanças não foram salvas.
          </p>
          <button type="button" autofocus (click)="reload()">Recarregar a ficha</button>
        }
      </dialog>

      <nav aria-label="Passos da criação do personagem">
        <ol>
          @for (step of steps; track step.path) {
            <li>
              <a [routerLink]="step.path" routerLinkActive="active" ariaCurrentWhenActive="step">
                {{ step.label }}
              </a>
            </li>
          }
        </ol>
      </nav>

      <router-outlet />
    } @else {
      <p>Carregando ficha…</p>
    }
  `,
  styles: `
    .bar {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem 1rem;
      align-items: center;
      margin-top: 1.25rem;
    }
    .bar label {
      font-family: var(--font-sc);
      color: var(--muted);
    }
    .bar input {
      flex: 0 1 18rem;
      font-family: var(--font-display);
      font-size: 1.4rem;
    }
    .errors {
      margin-top: 1rem;
      padding: 0.75rem 1rem;
      border: 1px solid var(--danger);
      border-radius: var(--radius);
      background: var(--danger-soft);
    }
    .errors p {
      margin: 0;
      font-weight: 600;
    }
    .errors ul {
      margin: 0.25rem 0 0;
    }
    .status {
      color: var(--success);
      font-size: 0.95em;
      font-style: italic;
    }
    .status.problem,
    .error {
      color: var(--danger);
    }
    .bar a {
      margin-left: auto;
    }
    /* Passos como capítulos: numeral romano rubricado, o atual sublinhado em vermelho. */
    nav {
      margin: 1rem 0 0.5rem;
      border-bottom: 1px solid var(--border);
    }
    nav ol {
      display: flex;
      flex-wrap: wrap;
      gap: 0 1.75rem;
      margin: 0;
      padding: 0;
      list-style: none;
      counter-reset: step;
    }
    nav li {
      counter-increment: step;
    }
    nav a {
      display: inline-flex;
      gap: 0.4rem;
      align-items: baseline;
      padding: 0.4rem 0 0.3rem;
      border-bottom: 3px solid transparent;
      margin-bottom: -1px;
      font-family: var(--font-sc);
      font-size: 1.1rem;
      color: var(--muted);
      text-decoration: none;
    }
    nav a::before {
      content: counter(step, upper-roman) '.';
      font-family: var(--font-display);
      color: var(--accent);
    }
    nav a:hover {
      color: var(--ink);
    }
    nav a.active {
      border-bottom-color: var(--accent);
      color: var(--ink);
    }
  `,
})
export class WizardShellComponent {
  /** `:id` da rota (withComponentInputBinding). */
  readonly id = input.required<string>();

  protected readonly store = inject(CharacterDraftStore);
  protected readonly steps = wizardSteps;
  /** Falha ao abrir: 404 e 422 não adiantam repetir; rede/5xx sim (F8 AC8). */
  protected readonly loadError = signal<{ message: string; canRetry: boolean } | null>(null);
  protected readonly nameError = signal<string | null>(null);
  /** Estados em que o salvamento não aconteceu: status em cor de alerta (F8 AC4). */
  protected readonly problem = computed(() =>
    ['invalid', 'conflict', 'error'].includes(this.store.status()),
  );
  /** Cada erro do último salvamento recusado, com o passo que o resolve (F8 AC1). */
  protected readonly errorList = computed(() =>
    Object.entries(this.store.errors()).flatMap(([field, messages]) =>
      messages.map((message) => ({ message, step: stepOfField(field) })),
    ),
  );

  private readonly conflictDialog = viewChild<ElementRef<HTMLDialogElement>>('conflictDialog');

  constructor() {
    afterRenderEffect(() => {
      const dialog = this.conflictDialog()?.nativeElement;
      const conflict = this.store.status() === 'conflict';
      if (dialog && conflict && !dialog.open) {
        dialog.showModal();
      } else if (dialog && !conflict && dialog.open) {
        dialog.close();
      }
    });
    effect(() => {
      const id = this.id();
      untracked(() => void this.load(id));
    });
  }

  /** Nome vazio não é salvo: o campo volta ao nome anterior e o motivo aparece (F8 AC6). */
  protected rename(event: Event): void {
    const input = event.target as HTMLInputElement;
    const name = input.value.trim();
    if (name) {
      this.nameError.set(null);
      this.store.rename(name);
    } else {
      input.value = this.store.name();
      this.nameError.set('O nome da ficha não pode ficar vazio.');
    }
  }

  /** Fechar/recarregar a aba antes do salvamento automático terminar pede confirmação (F8 AC12). */
  protected confirmLeave(event: BeforeUnloadEvent): void {
    if (this.store.status() === 'pending' || this.store.status() === 'saving') {
      event.preventDefault();
    }
  }

  protected async reload(): Promise<void> {
    await this.load(this.id());
  }

  private async load(id: string): Promise<void> {
    this.loadError.set(null);
    try {
      await this.store.open(id);
    } catch (e) {
      const status = e instanceof HttpErrorResponse ? e.status : 0;
      this.loadError.set({ message: apiFailure(e), canRetry: status !== 404 && status !== 422 });
    }
  }
}
