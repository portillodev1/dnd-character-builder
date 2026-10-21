import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { saved } from '../testing/fixtures';
import { CharacterDraftStore, SAVE_DEBOUNCE_MS } from './character-draft.store';

// Caminho relativo: a origem da API vem do proxy do ng serve / mesma origem (ADR-0010).
const url = '/api/v1/character-sheets/00000000-0000-0000-0000-000000000001';

describe('CharacterDraftStore', () => {
  let store: CharacterDraftStore;
  let http: HttpTestingController;

  beforeEach(async () => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    store = TestBed.inject(CharacterDraftStore);
    http = TestBed.inject(HttpTestingController);
    const opened = store.open('00000000-0000-0000-0000-000000000001');
    http.expectOne(url).flush(saved());
    await opened;
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
    http.verify();
  });

  it('update quando várias mudanças seguidas deve gravar uma vez só, com o rascunho mais recente (F5 AC3)', async () => {
    store.update({ raceId: 'anao' });
    store.update({ subraceId: 'anao-da-colina' });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);

    const req = http.expectOne(url);
    expect(req.request.body).toEqual({
      name: 'Thorin',
      version: 1,
      draft: { raceId: 'anao', subraceId: 'anao-da-colina' },
    });
    req.flush(saved({ version: 2 }));
    await store.flush();

    expect([store.status(), store.version(), store.savedAt()]).toEqual([
      'saved',
      2,
      expect.any(Date),
    ]);
  });

  it('save quando a API responde 409 deve marcar conflito e não gravar mais nada (F5 AC6)', async () => {
    store.update({ level: 2 });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);
    http.expectOne(url).flush({ title: 'Conflito' }, { status: 409, statusText: 'Conflict' });
    await store.flush();

    store.update({ level: 3 });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);

    expect(store.status()).toBe('conflict');
    http.expectNone(url);
  });

  it('save quando a API responde 400 deve guardar os erros por campo sem o prefixo draft.', async () => {
    store.update({ level: 30 });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);
    http.expectOne(url).flush(
      // chave como a API devolve no PUT (SheetInput.Validate → "draft.<campo>")
      { errors: { 'draft.level': ['O nível deve estar entre 1 e 20.'] } },
      { status: 400, statusText: 'Bad Request' },
    );
    await store.flush();

    expect([store.status(), store.errors()]).toEqual([
      'invalid',
      { level: ['O nível deve estar entre 1 e 20.'] },
    ]);
  });

  it('save quando a API não responde deve marcar erro com o motivo e "Tentar de novo" deve regravar (F8 AC4, AC5)', async () => {
    store.update({ level: 3 });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);
    http.expectOne(url).flush(null, { status: 503, statusText: 'Service Unavailable' });
    await store.flush();
    const failed = [store.status(), store.failure()];

    store.retry();
    http.expectOne(url).flush(saved({ version: 2, draft: { level: 3 } }));
    await store.flush();

    expect([failed, store.status(), store.failure()]).toEqual([
      ['error', 'A API (servidor) não respondeu. Tente de novo em instantes.'],
      'saved',
      null,
    ]);
  });
});
