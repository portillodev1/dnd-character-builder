import { expectAccessible, expect, standardArray, test } from './support';

// Critério 4 do MVP — spec .specs/features/bonus-breakdown, exemplo verificável AC7 (LdJ p. 20).
const hillDwarf = {
  raceId: 'anao',
  subraceId: 'anao-da-colina',
  classId: 'ladino',
  level: 1,
  ...standardArray([10, 15, 14, 12, 13, 8]),
};

test.describe('F4 de onde vem cada bônus', () => {
  test('Constituição 14 de Anão da Colina deve virar 16 com a conta', async ({ page, api }) => {
    const id = await api.create('Conta', hillDwarf);
    await page.goto(`/fichas/${id}/ficha`);

    const con = page.getByRole('button', { name: 'Constituição: 16. Ver a conta' });
    await con.focus();
    await page.keyboard.press('Enter');

    const panel = page.getByRole('region', { name: 'Conta de Constituição' });
    await expect(panel.getByText('Constituição = 16')).toBeVisible();
    await expect(panel.getByRole('listitem')).toHaveText([
      /\+14.*Valor base.*LdJ p\. \d+/,
      /\+2.*Anão.*Aumento de atributo.*LdJ p\. 20/,
    ]);
    await expect(
      page.getByRole('button', { name: 'Modificador de Constituição: +3. Ver a conta' }),
    ).toBeVisible();
  });

  test('Meus bônus deve agrupar por origem', async ({ page, api }) => {
    const id = await api.create('Origens', hillDwarf);
    await page.goto(`/fichas/${id}/ficha`);

    const mine = page.getByRole('region', { name: 'Meus bônus' });
    await expect(mine.getByRole('heading', { name: /^Raça/ })).toBeVisible();
    await expect(mine.getByRole('heading', { name: /^Sub-raça/ })).toBeVisible();
    await expect(mine.getByRole('heading', { name: /^Classe/ })).toBeVisible();
    await expect(mine.getByText(/\+2 Constituição · Aumento de atributo · Anão/)).toBeVisible();
  });

  test('legenda deve explicar cada tipo e a ficha deve ser acessível', async ({ page, api }) => {
    const id = await api.create('Legenda', hillDwarf);
    await page.goto(`/fichas/${id}/ficha`);
    await page.getByText('Legenda: tipos de bônus').click();

    await expect(page.getByText('Role 2d20 e fique com o maior resultado.')).toBeVisible();
    await expectAccessible(page);
  });
});
