import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { CharacterDraft, CharacterSheet, SavedSheet, ValidationProblem } from '../core/api/models';
import { SheetService } from '../core/api/sheet.service';

export type SaveStatus = 'idle' | 'pending' | 'saving' | 'saved' | 'invalid' | 'conflict' | 'error';

/** Espera após a última mudança antes de gravar o arquivo (F5 AC3). */
export const SAVE_DEBOUNCE_MS = 600;

/**
 * Ficha aberta no wizard. Guarda só as escolhas (`draft`); a ficha derivada (`sheet`) é sempre a
 * última resposta da API — o front não calcula nada (ADR-0002).
 */
@Injectable({ providedIn: 'root' })
export class CharacterDraftStore {
  private readonly api = inject(SheetService);
  private timer: ReturnType<typeof setTimeout> | undefined;
  private inFlight: Promise<void> | undefined;
  private dirty = false;

  readonly id = signal<string | null>(null);
  readonly name = signal('');
  readonly version = signal(0);
  readonly draft = signal<CharacterDraft>({});
  readonly sheet = signal<CharacterSheet | null>(null);
  /** Erros por campo do último 400 (o rascunho não foi gravado). */
  readonly errors = signal<Record<string, string[]>>({});
  readonly status = signal<SaveStatus>('idle');
  readonly savedAt = signal<Date | null>(null);
  readonly warnings = computed(() => this.sheet()?.warnings ?? []);

  async open(id: string): Promise<void> {
    clearTimeout(this.timer);
    this.dirty = false;
    this.apply(await this.api.get(id));
    this.status.set('idle');
    this.savedAt.set(null);
  }

  /** Aplica uma mudança no rascunho e agenda o salvamento automático. */
  update(change: Partial<CharacterDraft> | ((d: CharacterDraft) => CharacterDraft)): void {
    this.draft.update((d) => (typeof change === 'function' ? change(d) : { ...d, ...change }));
    this.schedule();
  }

  rename(name: string): void {
    this.name.set(name);
    this.schedule();
  }

  /** Grava já (sem esperar o debounce) e aguarda o término. */
  async flush(): Promise<void> {
    if (this.timer !== undefined) {
      clearTimeout(this.timer);
      this.timer = undefined;
      await this.save();
    }
    await this.inFlight;
  }

  private schedule(): void {
    if (this.status() === 'conflict') {
      return; // nada é sobrescrito em silêncio (F5 AC6)
    }
    this.status.set('pending');
    clearTimeout(this.timer);
    this.timer = setTimeout(() => {
      this.timer = undefined;
      void this.save();
    }, SAVE_DEBOUNCE_MS);
  }

  private async save(): Promise<void> {
    const id = this.id();
    if (id === null) {
      return;
    }
    if (this.inFlight) {
      this.dirty = true;
      return;
    }
    this.status.set('saving');
    this.inFlight = this.put(id);
    try {
      await this.inFlight;
    } finally {
      this.inFlight = undefined;
    }
    if (this.dirty && this.status() !== 'conflict') {
      this.dirty = false;
      await this.save();
    }
  }

  private async put(id: string): Promise<void> {
    try {
      this.apply(await this.api.update(id, this.version(), this.name(), this.draft()), false);
      this.status.set('saved');
      this.savedAt.set(new Date());
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.status === 409) {
        this.status.set('conflict');
      } else if (e instanceof HttpErrorResponse && e.status === 400) {
        this.errors.set((e.error as ValidationProblem | null)?.errors ?? {});
        this.status.set('invalid');
      } else {
        this.status.set('error');
      }
    }
  }

  private apply(saved: SavedSheet, replaceDraft = true): void {
    this.id.set(saved.id);
    this.version.set(saved.version);
    if (replaceDraft) {
      this.name.set(saved.name);
      this.draft.set(saved.draft);
    }
    this.sheet.set(saved.sheet);
    this.errors.set(saved.errors ?? {});
  }
}
