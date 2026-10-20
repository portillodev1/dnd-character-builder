import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api-base-url';
import { CharacterDraft, SavedSheet, SheetSummary } from './models';

/** Fichas salvas em arquivo no servidor (critério 5, ADR-0006). */
@Injectable({ providedIn: 'root' })
export class SheetService {
  private readonly http = inject(HttpClient);
  private readonly base = `${inject(API_BASE_URL)}/api/v1/character-sheets`;

  list(): Promise<SheetSummary[]> {
    return firstValueFrom(this.http.get<SheetSummary[]>(this.base));
  }

  get(id: string): Promise<SavedSheet> {
    return firstValueFrom(this.http.get<SavedSheet>(`${this.base}/${id}`));
  }

  create(name: string, draft: CharacterDraft = {}): Promise<SavedSheet> {
    return firstValueFrom(this.http.post<SavedSheet>(this.base, { name, draft }));
  }

  update(id: string, version: number, name: string, draft: CharacterDraft): Promise<SavedSheet> {
    return firstValueFrom(
      this.http.put<SavedSheet>(`${this.base}/${id}`, { name, version, draft }),
    );
  }

  delete(id: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.base}/${id}`));
  }
}
