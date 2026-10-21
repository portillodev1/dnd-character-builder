import { InjectionToken } from '@angular/core';

/**
 * Origem da API (.NET). Vazia = mesma origem do front: no dev o `ng serve` encaminha `/api` e
 * `/health` pelo `proxy.conf.json`; no E2E, pelo `proxy.e2e.conf.json` (ADR-0010).
 */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  providedIn: 'root',
  factory: () => '',
});
