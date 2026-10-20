import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api-base-url';
import { ClassFeatures, ClassSummary, Race, RaceSummary } from './models';

/** Catálogo de raças e classes (critérios 1 e 2). */
@Injectable({ providedIn: 'root' })
export class RulesetService {
  private readonly http = inject(HttpClient);
  private readonly base = `${inject(API_BASE_URL)}/api/v1`;

  races(): Promise<RaceSummary[]> {
    return firstValueFrom(this.http.get<RaceSummary[]>(`${this.base}/races`));
  }

  race(id: string): Promise<Race> {
    return firstValueFrom(this.http.get<Race>(`${this.base}/races/${encodeURIComponent(id)}`));
  }

  classes(): Promise<ClassSummary[]> {
    return firstValueFrom(this.http.get<ClassSummary[]>(`${this.base}/classes`));
  }

  classFeatures(id: string, level: number): Promise<ClassFeatures> {
    return firstValueFrom(
      this.http.get<ClassFeatures>(`${this.base}/classes/${encodeURIComponent(id)}/features`, {
        params: { level },
      }),
    );
  }
}
