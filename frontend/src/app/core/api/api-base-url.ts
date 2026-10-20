import { InjectionToken } from '@angular/core';

/** Origem da API (.NET). Em dev: `dotnet run --project backend/src/DndBuilder.Api`. */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  providedIn: 'root',
  factory: () => 'http://localhost:5080',
});
