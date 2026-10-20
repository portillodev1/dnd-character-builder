import { expectAccessible, expect, test } from './support';

// Critério 5 do MVP — spec .specs/features/character-sheets (ADR-0006).
test.describe('F5 Minhas fichas', () => {
  test('criar, salvar, reabrir e apagar uma ficha', async ({ page }) => {
    await page.goto('/fichas');
    await expectAccessible(page);

    await page.getByLabel('Nome do personagem').fill('Gimli');
    await page.getByRole('button', { name: 'Nova ficha' }).click();
    await expect(page).toHaveURL(/\/fichas\/[\w-]+\/raca$/);

    await page.getByRole('radio', { name: /^Anão Povo/ }).check();
    await expect(page.getByRole('status').filter({ hasText: /Salvo às \d\d:\d\d/ })).toBeVisible();

    await page.getByRole('link', { name: 'Minhas fichas' }).click();
    const item = page.getByRole('listitem').filter({ hasText: 'Gimli' });
    await expect(item).toContainText('Anão');

    await item.getByRole('link', { name: 'Gimli' }).click();
    await expect(page.getByRole('radio', { name: /^Anão Povo/ })).toBeChecked();

    await page.getByRole('link', { name: 'Minhas fichas' }).click();
    await page
      .getByRole('listitem')
      .filter({ hasText: 'Gimli' })
      .getByRole('button', { name: 'Apagar' })
      .click();
    await expect(page.getByRole('alertdialog')).toContainText('Apagar a ficha "Gimli"?');
    await page.getByRole('button', { name: 'Apagar ficha' }).click();
    await expect(page.getByRole('listitem').filter({ hasText: 'Gimli' })).toHaveCount(0);
  });

  test('salvamento recusado pela API deve mostrar o motivo no passo', async ({ page, api }) => {
    const id = await api.create('Pontos demais', {
      raceId: 'humano',
      classId: 'guerreiro',
      level: 1,
    });
    await page.goto(`/fichas/${id}/atributos`);
    await page.getByRole('radio', { name: 'Compra de pontos' }).check();

    // 15 custa 9 pontos (LdJ p. 13): 6 × 9 = 54, acima do orçamento de 27.
    for (const ability of [
      'Força',
      'Destreza',
      'Constituição',
      'Inteligência',
      'Sabedoria',
      'Carisma',
    ]) {
      const field = page.getByRole('spinbutton', { name: ability });
      await field.fill('15');
      await field.blur();
    }

    await expect(page.getByRole('status').filter({ hasText: 'Não salvo' })).toBeVisible();
    await expect(page.getByRole('alert')).toContainText('54 pontos gastos, o máximo é 27');
  });

  test('ficha alterada em outra aba deve oferecer recarregar', async ({ page, api, context }) => {
    const id = await api.create('Duas abas');
    await page.goto(`/fichas/${id}/raca`);
    const other = await context.newPage();
    await other.goto(`/fichas/${id}/raca`);

    await other.getByRole('radio', { name: /^Humano/ }).check();
    await expect(other.getByRole('status').filter({ hasText: /Salvo às/ })).toBeVisible();
    await page.getByRole('radio', { name: /^Halfling/ }).check();

    await expect(page.getByRole('alertdialog')).toContainText('alterada em outra aba');
    await page.getByRole('button', { name: 'Recarregar a ficha' }).click();
    await expect(page.getByRole('radio', { name: /^Humano/ })).toBeChecked();
  });
});
