import { HttpErrorResponse } from '@angular/common/http';

const OFFLINE = 'A API (servidor) não respondeu. Tente de novo em instantes.';

/**
 * Texto para o jogador a partir de uma falha de chamada à API (F8 AC5, AC8, AC9). Distingue a
 * ficha que não existe (404) e o arquivo com problema (422, F7) de rede fora / erro do servidor.
 * O 400 de escolhas inválidas não passa por aqui: vira erros por campo no store.
 */
export function apiFailure(e: unknown): string {
  if (!(e instanceof HttpErrorResponse)) {
    return OFFLINE;
  }
  if (e.status === 404) {
    return 'Ficha não encontrada: ela pode ter sido apagada.';
  }
  if (e.status === 422) {
    const detail = (e.error as { detail?: string } | null)?.detail;
    return `O arquivo desta ficha está com problema${detail ? `: ${detail}` : '.'}`;
  }
  return OFFLINE;
}
