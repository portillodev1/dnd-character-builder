import { HttpErrorResponse } from '@angular/common/http';
import { apiFailure } from './api-failure';

const failure = (status: number, error: unknown = null) =>
  apiFailure(new HttpErrorResponse({ status, error }));

describe('apiFailure', () => {
  it('quando 404 deve dizer que a ficha não existe; 422 deve trazer o motivo do arquivo (F8 AC8)', () => {
    expect([failure(404), failure(422, { detail: 'abc.json: falta o campo "draft".' })]).toEqual([
      'Ficha não encontrada: ela pode ter sido apagada.',
      'O arquivo desta ficha está com problema: abc.json: falta o campo "draft".',
    ]);
  });

  it('quando rede fora ou 5xx deve dizer que o servidor não respondeu (F8 AC5)', () => {
    expect(new Set([failure(0), failure(500), apiFailure(new Error('x'))])).toEqual(
      new Set(['A API (servidor) não respondeu. Tente de novo em instantes.']),
    );
  });
});
