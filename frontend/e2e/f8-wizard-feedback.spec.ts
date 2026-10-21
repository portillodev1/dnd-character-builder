import { expectAccessible, expect, standardArray, test } from './support';

// Spec F8 — .specs/features/wizard-feedback (Fase 7).
// Elfo da Floresta (Percepção racial) Ladino nv. 6: 4 Especializações (LdJ p. 91).
const rogue6 = {
  raceId: 'elfo',
  subraceId: 'elfo-da-floresta',
  classId: 'ladino',
  level: 6,
  ...standardArray([8, 15, 13, 12, 14, 10]),
  skillChoices: ['furtividade', 'acrobacia', 'enganacao', 'investigacao'],
  extraProficiencies: ['atletismo', 'intuicao'],
  expertise: ['furtividade', 'acrobacia', 'enganacao', 'percepcao'],
};

test.describe('F8 o wizard diz o que está acontecendo', () => {
  test('baixar o nível deve desmarcar Especializações excedentes e continuar salvando', async ({
    page,
    api,
  }) => {
    const id = await api.create('Nível menor', rogue6);
    await page.goto(`/fichas/${id}/classe`);

    await page.getByLabel('Nível do personagem').selectOption('3');

    await expect(page.getByRole('status').filter({ hasText: /Salvo às/ })).toBeVisible();
    await expect(page.getByText('desmarcamos Enganação e Percepção')).toBeVisible();
    await expectAccessible(page);
  });

  test('API fora do ar deve ser explicada na lista e ao abrir a ficha, com "Tentar de novo"', async ({
    page,
    api,
  }) => {
    const id = await api.create('Sem servidor');
    await page.route('**/api/**', (route) => route.abort());

    await page.goto('/fichas');
    await expect(page.getByRole('alert')).toContainText('a API (servidor) não respondeu');

    await page.goto(`/fichas/${id}/raca`);
    const alert = page.getByRole('alert');
    await expect(alert).toContainText('A API (servidor) não respondeu');
    await expect(alert.getByRole('button', { name: 'Tentar de novo' })).toBeVisible();
    await expectAccessible(page);
  });

  test('diálogo de apagar deve receber o foco, prender o Tab e fechar com Esc', async ({
    page,
    api,
  }) => {
    await api.create('Diálogo por teclado');
    await page.goto('/fichas');
    const remove = page
      .getByRole('listitem')
      .filter({ has: page.getByRole('link', { name: 'Diálogo por teclado', exact: true }) })
      .getByRole('button', { name: 'Apagar' });

    await remove.focus();
    await page.keyboard.press('Enter');
    const dialog = page.getByRole('alertdialog');
    await expect(dialog.getByRole('button', { name: 'Cancelar' })).toBeFocused();
    await expectAccessible(page);

    await page.keyboard.press('Shift+Tab');
    await expect(dialog.getByRole('button', { name: 'Apagar ficha' })).toBeFocused();

    await page.keyboard.press('Escape');
    await expect(dialog).toBeHidden();
    await expect(remove).toBeFocused();
    await expect(
      page.getByRole('link', { name: 'Diálogo por teclado', exact: true }),
    ).toBeVisible();
  });
});
