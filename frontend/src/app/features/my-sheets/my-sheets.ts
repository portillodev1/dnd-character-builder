import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnInit,
  afterRenderEffect,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { SheetSummary } from '../../core/api/models';
import { RulesetService } from '../../core/api/ruleset.service';
import { apiFailure } from '../../core/api/api-failure';
import { SheetService } from '../../core/api/sheet.service';

/** "Minhas fichas": listar, criar, abrir, duplicar, apagar (F5 AC1, AC2, AC7; F7 AC5). */
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
              {{ describe(s) }} · alterada em {{ s.updatedAt | date: "dd/MM/yyyy 'às' HH:mm" }}
            </span>
          }
          @if (!s.error) {
            <button type="button" (click)="duplicate(s)">Duplicar</button>
          }
          <button type="button" (click)="confirming.set(s)">Apagar</button>
        </li>
      }
    </ul>

    <!-- Modal nativo: foco preso, Esc = Cancelar, foco volta ao botão de origem (F8 AC13). -->
    <dialog
      #confirmDialog
      role="alertdialog"
      aria-labelledby="confirm-title"
      class="confirm"
      (close)="confirming.set(null)"
    >
      @if (confirming(); as s) {
        <p id="confirm-title">Apagar a ficha "{{ s.name ?? s.id }}"? Isso não pode ser desfeito.</p>
        <button type="button" (click)="remove(s)">Apagar ficha</button>
        <button type="button" autofocus (click)="confirmDialog.close()">Cancelar</button>
      }
    </dialog>
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
      border: var(--rule);
      border-radius: var(--radius);
      background: var(--surface);
    }
    .list a {
      font-family: var(--font-display);
      font-size: 1.4rem;
    }
    /* Duplicar e Apagar juntos à direita da linha. */
    .list button:first-of-type {
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
  private readonly ruleset = inject(RulesetService);

  protected newName = '';
  protected readonly sheets = signal<SheetSummary[]>([]);
  protected readonly loaded = signal(false);
  protected readonly confirming = signal<SheetSummary | null>(null);
  protected readonly error = signal<string | null>(null);
  /** id → nome de raças e classes do catálogo, para a lista não mostrar ids. */
  private readonly names = signal<ReadonlyMap<string, string>>(new Map());

  private readonly confirmDialog =
    viewChild.required<ElementRef<HTMLDialogElement>>('confirmDialog');

  constructor() {
    // Depois de renderizar o conteúdo, para o autofocus de "Cancelar" valer.
    afterRenderEffect(() => {
      const dialog = this.confirmDialog().nativeElement;
      if (this.confirming() && !dialog.open) {
        dialog.showModal();
      } else if (!this.confirming() && dialog.open) {
        dialog.close();
      }
    });
  }

  async ngOnInit(): Promise<void> {
    await Promise.all([this.load(), this.loadNames()]);
  }

  /** "Anão · Ladino · nível 3"; o que ainda não foi escolhido fica de fora. */
  protected describe(s: SheetSummary): string {
    const name = (id: string | null | undefined) => (id ? (this.names().get(id) ?? id) : null);
    const race = name(s.raceId);
    const cls = name(s.classId);
    if (!race && !cls) {
      return 'Raça e classe a escolher';
    }
    return [
      race ?? 'raça a escolher',
      cls ? `${cls} · nível ${s.level ?? 'a escolher'}` : 'classe a escolher',
    ].join(' · ');
  }

  protected async create(): Promise<void> {
    await this.attempt(async () => {
      const saved = await this.api.create(this.newName.trim());
      await this.router.navigate(['/fichas', saved.id, 'raca']);
    });
  }

  protected async duplicate(sheet: SheetSummary): Promise<void> {
    await this.attempt(async () => {
      const copy = await this.api.duplicate(sheet.id);
      await this.router.navigate(['/fichas', copy.id, 'raca']);
    });
  }

  protected async remove(sheet: SheetSummary): Promise<void> {
    this.confirming.set(null);
    await this.attempt(async () => {
      await this.api.delete(sheet.id);
      this.sheets.update((list) => list.filter((s) => s.id !== sheet.id));
    });
  }

  /** Falha de criar/duplicar/apagar vira aviso em `role="alert"`; o botão continua usável (F8 AC9). */
  private async attempt(action: () => Promise<void>): Promise<void> {
    this.error.set(null);
    try {
      await action();
    } catch (e) {
      this.error.set(apiFailure(e));
    }
  }

  private async loadNames(): Promise<void> {
    try {
      const [races, classes] = await Promise.all([this.ruleset.races(), this.ruleset.classes()]);
      this.names.set(new Map([...races, ...classes].map((x) => [x.id, x.name])));
    } catch {
      // Sem catálogo a lista continua útil com os ids; o erro de API já aparece em load().
    }
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
        'Não foi possível carregar suas fichas: a API (servidor) não respondeu. Confira se ela está rodando.',
      );
    } finally {
      this.loaded.set(true);
    }
  }
}
