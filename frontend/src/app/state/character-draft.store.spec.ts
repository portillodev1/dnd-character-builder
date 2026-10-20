import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { saved } from '../testing/fixtures';
import { CharacterDraftStore, SAVE_DEBOUNCE_MS } from './character-draft.store';

const url = 'http://localhost:5080/api/v1/character-sheets/00000000-0000-0000-0000-000000000001';

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

  it('save quando a API responde 400 deve guardar os erros por campo', async () => {
    store.update({ level: 30 });
    vi.advanceTimersByTime(SAVE_DEBOUNCE_MS);
    http
      .expectOne(url)
      .flush(
        { errors: { level: ['O nível deve estar entre 1 e 20.'] } },
        { status: 400, statusText: 'Bad Request' },
      );
    await store.flush();

    expect([store.status(), store.errors()]).toEqual([
      'invalid',
      { level: ['O nível deve estar entre 1 e 20.'] },
    ]);
  });
});
