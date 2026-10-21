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

  test('Ladino nível 4 deve marcar o que o app ainda não aplica e avisar na ficha', async ({
    page,
    api,
  }) => {
    const id = await api.create('Ladino 4', { raceId: 'humano', classId: 'ladino', level: 4 });
    await page.goto(`/fichas/${id}/classe`);

    const asi = page.getByRole('article').filter({
      has: page.getByRole('heading', { name: /Incremento no Valor de Habilidade/ }),
    });
    await expect(asi.first()).toContainText('Ainda não aplicado nesta ficha — anote à mão.');
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(
      page.getByText('Incremento no Valor de Habilidade (nível 4) não está somado aos atributos'),
    ).toBeVisible();
  });
});
