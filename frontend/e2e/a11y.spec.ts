import { expectAccessible, expect, standardArray, test } from './support';

// WCAG 2.2 AA (rules/frontend-angular.md): telas e estados que os testes F1–F5 não cobrem.
const complete = {
  raceId: 'meio-elfo',
  classId: 'bardo',
  level: 3,
  ...standardArray([8, 14, 13, 12, 10, 15]),
};

test.describe('acessibilidade', () => {
  test('passo Atributos deve ser acessível nos três métodos', async ({ page, api }) => {
    const id = await api.create('Atributos', complete);
    await page.goto(`/fichas/${id}/atributos`);
    await expectAccessible(page);

    await page.getByRole('radio', { name: 'Compra de pontos' }).check();
    await expectAccessible(page);
    await page.getByRole('radio', { name: 'Manual (rolagem)' }).check();
    await expectAccessible(page);
  });

  test('conta aberta e escolhas da raça devem ser acessíveis', async ({ page, api }) => {
    const id = await api.create('Conta aberta', complete);
    await page.goto(`/fichas/${id}/raca`);
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await page.getByRole('button', { name: /^Pontos de Vida: \d+\. Ver a conta$/ }).click();
    await expect(page.getByRole('region', { name: 'Conta de Pontos de Vida' })).toBeVisible();
    await expectAccessible(page);
  });

  test('wizard deve ser navegável só pelo teclado', async ({ page, api }) => {
    const id = await api.create('Teclado', complete);
    await page.goto(`/fichas/${id}/raca`);

    const steps = page.getByRole('navigation', { name: 'Passos da criação do personagem' });
    await steps.getByRole('link', { name: 'Classe' }).focus();
    await page.keyboard.press('Enter');
    await expect(page).toHaveURL(/\/classe$/);
    await expect(steps.getByRole('link', { name: 'Classe' })).toHaveAttribute(
      'aria-current',
      'step',
    );
  });
});

test.describe('acessibilidade no tema escuro', () => {
  test.use({ colorScheme: 'dark' });

  test('passos e ficha devem manter contraste à luz de vela', async ({ page, api }) => {
    const id = await api.create('Escuro', complete);
    for (const step of ['raca', 'classe', 'atributos', 'pericias', 'ficha']) {
      await page.goto(`/fichas/${id}/${step}`);
      await expect(page.getByRole('heading', { level: 2 })).toBeVisible();
      await expectAccessible(page);
    }
    await page.goto('/fichas');
    await expectAccessible(page);
  });
});
