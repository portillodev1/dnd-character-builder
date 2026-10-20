import AxeBuilder from '@axe-core/playwright';
import { test as base, expect, type APIRequestContext, type Page } from '@playwright/test';
import { E2E_API } from '../playwright.config';

/** Origem da API que o front conhece (`API_BASE_URL`); o E2E a redireciona para a API isolada. */
const APP_API = 'http://localhost:5080';

/** Rascunho no formato de `POST /character-sheets/compute` (docs/architecture/api-contract.md). */
export type Draft = Record<string, unknown>;

export const test = base.extend<{ api: SheetsApi }>({
  context: async ({ context }, use) => {
    // Vale para todas as abas do contexto (o teste de conflito abre duas).
    await context.route(`${APP_API}/**`, async (route) => {
      const url = route.request().url().replace(APP_API, E2E_API);
      await route.fulfill({ response: await route.fetch({ url }) });
    });
    await use(context);
  },
  api: async ({ playwright }, use) => {
    const ctx = await playwright.request.newContext({ baseURL: E2E_API });
    await use(new SheetsApi(ctx));
    await ctx.dispose();
  },
});

export { expect };

/** Prepara fichas direto pela API, para cada teste começar no passo que importa. */
export class SheetsApi {
  constructor(private readonly ctx: APIRequestContext) {}

  async create(name: string, draft: Draft = {}): Promise<string> {
    const res = await this.ctx.post('/api/v1/character-sheets', { data: { name, draft } });
    expect(res.status()).toBe(201);
    return ((await res.json()) as { id: string }).id;
  }
}

/** Atributos pelo conjunto padrão, na ordem For, Des, Con, Int, Sab, Car. */
export function standardArray(
  values: [number, number, number, number, number, number],
): Pick<Draft, 'abilityMethod' | 'baseAbilities'> {
  const [forca, destreza, constituicao, inteligencia, sabedoria, carisma] = values;
  return {
    abilityMethod: 'standard-array',
    baseAbilities: { forca, destreza, constituicao, inteligencia, sabedoria, carisma },
  };
}

/** WCAG 2.2 AA (rules/frontend-angular.md): nenhuma violação na página atual. */
export async function expectAccessible(page: Page): Promise<void> {
  const { violations } = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa'])
    .analyze();
  const summary = violations.map(
    (v) =>
      `${v.id} (${v.impact}): ${v.nodes.map((n) => `${n.target.join(' ')} — ${n.failureSummary}`).join('; ')}`,
  );
  expect(summary, 'violações de acessibilidade').toEqual([]);
}
