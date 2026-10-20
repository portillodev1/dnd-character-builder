import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { SheetSummary } from '../../core/api/models';
import { SheetService } from '../../core/api/sheet.service';

/** "Minhas fichas": listar, criar, abrir, apagar (F5 AC1, AC2, AC7). */
@Component({
  selector: 'app-my-sheets',
  imports: [DatePipe, FormsModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2>Minhas fichas</h2>

    <form (ngSubmit)="create()" class="new">
      <label for="new-sheet-name">Nome do personagem</label>
      <input id="new-sheet-name" name="name" [(ngModel)]="newName" required maxlength="80" />
      <button type="submit" [disabled]="!newName.trim()">Nova ficha</button>
    </form>

    @if (error()) {
      <p role="alert">{{ error() }}</p>
    }

    @if (sheets().length === 0 && loaded()) {
      <p>Nenhuma ficha ainda. Dê um nome e clique em "Nova ficha".</p>
    }
    <ul class="list">
      @for (s of sheets(); track s.id) {
        <li>
          @if (s.error) {
            <span>{{ s.name ?? s.id }} — arquivo com problema: {{ s.error }}</span>
          } @else {
            <a [routerLink]="['/fichas', s.id]">{{ s.name }}</a>
            <span class="meta">
              {{ s.raceId ?? 'sem raça' }} · {{ s.classId ?? 'sem classe' }} · nível
              {{ s.level ?? '—' }} · alterada em {{ s.updatedAt | date: 'dd/MM/yyyy HH:mm' }}
            </span>
          }
          <button type="button" (click)="confirming.set(s)">Apagar</button>
        </li>
      }
    </ul>

    @if (confirming(); as s) {
      <div role="alertdialog" aria-modal="true" aria-labelledby="confirm-title" class="confirm">
        <p id="confirm-title">Apagar a ficha "{{ s.name ?? s.id }}"? Isso não pode ser desfeito.</p>
        <button type="button" (click)="remove(s)">Apagar ficha</button>
        <button type="button" (click)="confirming.set(null)">Cancelar</button>
      </div>
    }
  `,
  styles: `
    .new {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem 0.75rem;
      align-items: center;
      padding: 1rem;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .new label {
      font-weight: 600;
    }
    .new input {
      flex: 1 1 14rem;
    }
    .list {
      display: grid;
      gap: 0.5rem;
      padding: 0;
      list-style: none;
    }
    .list li {
      display: flex;
      flex-wrap: wrap;
      gap: 0.25rem 1rem;
      align-items: center;
      padding: 0.75rem 1rem;
      border: 1px solid var(--border);
      border-left: 4px solid var(--accent);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .list a {
      font-family: var(--font-display);
      font-size: 1.15rem;
      font-weight: 700;
    }
    .list button {
      margin-left: auto;
    }
    .meta {
      color: var(--muted);
      font-size: 0.9em;
    }
    .confirm button:first-of-type {
      border-color: var(--danger);
      background: var(--danger);
      color: var(--surface);
    }
  `,
})
export class MySheetsComponent implements OnInit {
  private readonly api = inject(SheetService);
  private readonly router = inject(Router);

  protected newName = '';
  protected readonly sheets = signal<SheetSummary[]>([]);
  protected readonly loaded = signal(false);
  protected readonly confirming = signal<SheetSummary | null>(null);
  protected readonly error = signal<string | null>(null);

  async ngOnInit(): Promise<void> {
    await this.load();
  }

  protected async create(): Promise<void> {
    const saved = await this.api.create(this.newName.trim());
    await this.router.navigate(['/fichas', saved.id, 'raca']);
  }

  protected async remove(sheet: SheetSummary): Promise<void> {
    this.confirming.set(null);
    await this.api.delete(sheet.id);
    this.sheets.update((list) => list.filter((s) => s.id !== sheet.id));
  }

  private async load(): Promise<void> {
    try {
      const list = await this.api.list();
      // mais recente primeiro (F5 AC1)
      this.sheets.set(
        [...list].sort((a, b) => (b.updatedAt ?? '').localeCompare(a.updatedAt ?? '')),
      );
    } catch {
      this.error.set(
        'Não foi possível falar com a API. Ela está rodando em http://localhost:5080?',
      );
    } finally {
      this.loaded.set(true);
    }
  }
}
