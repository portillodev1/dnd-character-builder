import { expectAccessible, expect, test } from './support';

// Critério 2 do MVP — spec .specs/features/class-features (LdJ cap. 3, Ladino p. 94–97).
test.describe('F2 características de classe', () => {
  test('passo Classe deve listar as 12 classes', async ({ page, api }) => {
    const id = await api.create('Classes');
    await page.goto(`/fichas/${id}/classe`);

    const classes = page.getByRole('group', { name: 'Escolha a classe' }).getByRole('radio');
    await expect(classes).toHaveCount(12);
    await expect(
      page.getByText(
        'Dado de Vida d8 · Atributo principal: Destreza · Salvaguardas: Destreza, Inteligência',
      ),
    ).toBeVisible();
    await expectAccessible(page);
  });

  test('Ladino nível 1 deve mostrar só características do nível 1', async ({ page, api }) => {
    const id = await api.create('Ladino 1', { classId: 'ladino', level: 1 });
    await page.goto(`/fichas/${id}/classe`);

    const now = page.getByRole('region', { name: 'Características até o nível 1' });
    for (const name of ['Especialização', 'Ataque Furtivo', 'Gíria de Ladrão']) {
      await expect(now.getByRole('heading', { name: new RegExp(`^${name}`) })).toBeVisible();
    }
    await expect(now.getByRole('heading', { name: /^Ação Ardilosa/ })).toHaveCount(0);
    const next = page.getByRole('region', { name: 'Próximo nível (2)' });
    await expect(next.getByRole('heading', { name: /^Ação Ardilosa/ })).toBeVisible();
  });

  test('mudar para nível 3 deve incluir Ação Ardilosa e o arquétipo', async ({ page, api }) => {
    const id = await api.create('Ladino 3', { classId: 'ladino', level: 1 });
    await page.goto(`/fichas/${id}/classe`);
    await page.getByLabel('Nível do personagem').selectOption('3');

    const now = page.getByRole('region', { name: 'Características até o nível 3' });
    await expect(now.getByRole('heading', { name: /^Ação Ardilosa/ })).toBeVisible();
    await expect(now.getByRole('heading', { name: /^Arquétipo de Ladino/ })).toBeVisible();
    await expectAccessible(page);
  });
});
