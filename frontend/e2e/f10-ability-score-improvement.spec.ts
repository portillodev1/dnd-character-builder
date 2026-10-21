import { expectAccessible, expect, standardArray, test } from './support';

// Spec F10 — .specs/features/ability-score-improvement (Fase 8).
// Humano (+1 em tudo, LdJ p. 31) Guerreiro nv. 4: Força 15 + 1 = 16; Incremento +2 → 18 (p. 85).
test.describe('F10 Incremento no Valor de Habilidade', () => {
  test('escolher +2 em Força deve somar na conta, aparecer em "Meus bônus" e sair ao baixar o nível', async ({
    page,
    api,
  }) => {
    const id = await api.create('Incremento', {
      raceId: 'humano',
      classId: 'guerreiro',
      level: 4,
      ...standardArray([15, 14, 13, 12, 10, 8]),
    });
    await page.goto(`/fichas/${id}/atributos`);
    const asi = page.getByRole('group', { name: /^Incremento no Valor de Habilidade \(nível 4\)/ });
    await asi.getByLabel('Primeiro +1').selectOption('forca');
    await asi.getByLabel('Segundo +1').selectOption('forca');

    await expect(page.getByRole('status').filter({ hasText: /Salvo às/ })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Força: 18. Ver a conta' })).toBeVisible();
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(
      page.getByText(
        '+1 Força · Aumento de atributo · Incremento no Valor de Habilidade (nível 4) (Guerreiro)',
      ),
    ).toHaveCount(2);

    await page.goto(`/fichas/${id}/classe`);
    await page.getByLabel('Nível do personagem').selectOption('3');
    await expect(
      page.getByText('saem as escolhas de Incremento no Valor de Habilidade (nível 4)'),
    ).toBeVisible();
    await expect(page.getByRole('status').filter({ hasText: /Salvo às/ })).toBeVisible();
  });
});
