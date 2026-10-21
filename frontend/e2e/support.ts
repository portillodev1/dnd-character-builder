import AxeBuilder from '@axe-core/playwright';
import { test as base, expect, type APIRequestContext, type Page } from '@playwright/test';
import { E2E_API } from '../playwright.config';

/** Rascunho no formato de `POST /character-sheets/compute` (docs/architecture/api-contract.md). */
export type Draft = Record<string, unknown>;

// O front chama a API por caminho relativo e o `ng serve` do E2E encaminha para a API isolada
// (ADR-0010): não há reescrita de rotas aqui.
export const test = base.extend<{ api: SheetsApi }>({
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
