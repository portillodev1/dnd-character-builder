import { expectAccessible, expect, test } from './support';

// Spec F9 — .specs/features/beginner-guidance (Fase 7).
test.describe('F9 guias para o iniciante', () => {
  test('compra de pontos deve mostrar quanto do orçamento já foi gasto', async ({ page, api }) => {
    // 13, 13, 13, 12, 12, 12 = 5 × 3 + 4 × 3 = 27 pontos (LdJ p. 13).
    const id = await api.create('Pontos', {
      raceId: 'humano',
      classId: 'guerreiro',
      level: 1,
      abilityMethod: 'point-buy',
      baseAbilities: {
        forca: 13,
        destreza: 13,
        constituicao: 13,
        inteligencia: 12,
        sabedoria: 12,
        carisma: 12,
      },
    });
    await page.goto(`/fichas/${id}/atributos`);
    await expect(page.getByText('Pontos gastos: 27 de 27 (restam 0).')).toBeVisible();

    const forca = page.getByRole('spinbutton', { name: 'Força' });
    await forca.fill('12');
    await forca.blur();

    await expect(page.getByText('Pontos gastos: 26 de 27 (restam 1).')).toBeVisible();
    await expectAccessible(page);
  });

  // F9 AC3/AC4 com F10 AC4: o Incremento passou a ser aplicado; o Arquétipo segue manual.
  test('Ladino nível 4 deve marcar o Arquétipo como "anote à mão" e avisar na ficha', async ({
    page,
    api,
  }) => {
    const id = await api.create('Ladino 4', { raceId: 'humano', classId: 'ladino', level: 4 });
    await page.goto(`/fichas/${id}/classe`);

    const card = (name: RegExp) =>
      page
        .getByRole('article')
        .filter({ has: page.getByRole('heading', { name }) })
        .first();
    await expect(card(/^Arquétipo de Ladino/)).toContainText('anote à mão');
    await expect(card(/^Incremento no Valor de Habilidade/)).not.toContainText('anote à mão');
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(page.getByText('Arquétipo de Ladino (nível 3): escolha à mão')).toBeVisible();
  });
});
