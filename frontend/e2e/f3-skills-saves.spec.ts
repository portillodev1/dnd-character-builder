import { expectAccessible, expect, standardArray, test } from './support';

// Critério 3 do MVP — spec .specs/features/skills-saves (LdJ p. 176 e 181).
// Elfo da Floresta (+2 Des, Percepção racial) Ladino nv. 1, Destreza base 14 → 16.
const elfRogue = {
  raceId: 'elfo',
  subraceId: 'elfo-da-floresta',
  classId: 'ladino',
  level: 1,
  ...standardArray([8, 14, 13, 12, 15, 10]),
};

test.describe('F3 perícias e salvaguardas', () => {
  test('Furtividade proficiente com Destreza 16 deve valer +5', async ({ page, api }) => {
    const id = await api.create('Furtiva', elfRogue);
    await page.goto(`/fichas/${id}/pericias`);
    await page.getByRole('checkbox', { name: 'Perícia de classe: Furtividade' }).check();

    await expect(page.getByRole('button', { name: 'Furtividade: +5. Ver a conta' })).toBeVisible();
    await expect(page.getByText(/Perícias do Ladino: 1 de 4/)).toBeVisible();
    await expectAccessible(page);
  });

  test('perícia fora da lista do Ladino deve ficar travada com motivo', async ({ page, api }) => {
    const id = await api.create('Lista', elfRogue);
    await page.goto(`/fichas/${id}/pericias`);

    await expect(
      page.getByRole('checkbox', { name: 'Perícia de classe: Arcanismo' }),
    ).toBeDisabled();
    await expect(page.getByText('Fora da lista do Ladino.').first()).toBeVisible();
  });

  test('Percepção racial do Elfo deve vir marcada e travada', async ({ page, api }) => {
    const id = await api.create('Racial', elfRogue);
    await page.goto(`/fichas/${id}/pericias`);

    const row = page.getByRole('row', { name: /Percepção \(Sab\)/ });
    await expect(
      row.getByRole('checkbox', { name: 'Perícia de classe: Percepção' }),
    ).toBeDisabled();
    await expect(row.getByText('Já proficiente por Elfo.')).toBeVisible();
  });

  test('salvaguardas do Ladino devem somar proficiência em Des e Int', async ({ page, api }) => {
    const id = await api.create('Salvas', elfRogue);
    await page.goto(`/fichas/${id}/pericias`);

    const saves = page.getByRole('table', { name: 'Salvaguardas' });
    await expect(saves.getByRole('row')).toHaveCount(6);
    // Destreza 16 → +3, + proficiência +2 (LdJ p. 179).
    await expect(
      saves.getByRole('button', { name: 'Salvaguarda de Destreza: +5. Ver a conta' }),
    ).toBeVisible();
    await expect(
      page.getByRole('heading', { name: 'Como preencher na ficha de papel' }),
    ).toBeVisible();
  });
});
