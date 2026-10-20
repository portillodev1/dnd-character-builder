import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  effect,
  inject,
  input,
  signal,
  untracked,
} from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { wizardSteps } from './wizard-steps';

/** Ficha aberta: nome, passos, estado do salvamento automático e conflito de versão (F5 AC3–AC6). */
@Component({
  selector: 'app-wizard-shell',
  imports: [DatePipe, RouterLink, RouterLinkActive, RouterOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (loadError()) {
      <p role="alert">
        Ficha não encontrada. <a routerLink="/fichas">Voltar para Minhas fichas</a>
      </p>
    } @else if (store.id()) {
      <div class="bar">
        <label for="sheet-name">Nome</label>
        <input id="sheet-name" [value]="store.name()" (change)="rename($event)" maxlength="80" />
        <span class="status" role="status" aria-live="polite">
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
              Não salvo: corrija os campos destacados.
            }
            @case ('error') {
              Não foi possível salvar. Verifique a API.
            }
          }
        </span>
        <a routerLink="/fichas">Minhas fichas</a>
      </div>

      @if (store.status() === 'conflict') {
        <div role="alertdialog" aria-modal="true" aria-labelledby="conflict-title" class="conflict">
          <p id="conflict-title">
            Esta ficha foi alterada em outra aba. Suas últimas mudanças não foram salvas.
          </p>
          <button type="button" (click)="reload()">Recarregar a ficha</button>
        </div>
      }

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
    .status {
      color: var(--success);
      font-size: 0.95em;
      font-style: italic;
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
  protected readonly loadError = signal(false);

  constructor() {
    effect(() => {
      const id = this.id();
      untracked(() => void this.load(id));
    });
  }

  protected rename(event: Event): void {
    const name = (event.target as HTMLInputElement).value.trim();
    if (name) {
      this.store.rename(name);
    }
  }

  protected async reload(): Promise<void> {
    await this.load(this.id());
  }

  private async load(id: string): Promise<void> {
    this.loadError.set(false);
    try {
      await this.store.open(id);
    } catch {
      this.loadError.set(true);
    }
  }
}
