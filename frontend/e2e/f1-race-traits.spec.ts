import { expectAccessible, expect, test } from './support';

// Critério 1 do MVP — spec .specs/features/race-traits (LdJ cap. 2).
test.describe('F1 traços de raça', () => {
  test('passo Raça quando abre deve listar as 9 raças com aumentos', async ({ page, api }) => {
    const id = await api.create('Raças');
    await page.goto(`/fichas/${id}/raca`);

    const races = page.getByRole('group', { name: 'Escolha a raça' }).getByRole('radio');
    await expect(races).toHaveCount(9);
    await expect(page.getByText('+2 Constituição').first()).toBeVisible();
    await expectAccessible(page);
  });

  test('Anão quando sem sub-raça deve bloquear o avanço', async ({ page, api }) => {
    const id = await api.create('Sem sub-raça', { raceId: 'anao' });
    await page.goto(`/fichas/${id}/raca`);

    await expect(page.getByText('Escolha uma sub-raça de Anão.')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Próximo' })).toBeDisabled();
  });

  test('Anão da Colina deve agrupar traços por origem', async ({ page, api }) => {
    const id = await api.create('Thorin', { raceId: 'anao' });
    await page.goto(`/fichas/${id}/raca`);
    await page.getByRole('radio', { name: /^Anão da Colina/ }).check();

    const anao = page.getByRole('region', { name: 'Traços de Anão', exact: true });
    const colina = page.getByRole('region', { name: 'Traços de Anão da Colina' });
    await expect(anao.getByRole('heading', { name: 'Visão no Escuro' })).toBeVisible();
    await expect(anao.getByRole('heading', { name: 'Resiliência Anã' })).toBeVisible();
    await expect(colina.getByRole('heading', { name: 'Tenacidade Anã' })).toBeVisible();
    await expect(anao.getByText(/LdJ p\. \d+/).first()).toBeVisible();
    await expectAccessible(page);
  });

  test('Meio-Elfo quando escolha pendente deve bloquear até completar', async ({ page, api }) => {
    const id = await api.create('Escolhas', { raceId: 'meio-elfo' });
    await page.goto(`/fichas/${id}/raca`);

    await expect(page.getByRole('button', { name: 'Próximo' })).toBeDisabled();
    const increase = page.getByRole('group', { name: /Aumento de Atributo/ });
    await increase.getByRole('checkbox', { name: 'Força' }).check();
    await increase.getByRole('checkbox', { name: 'Destreza' }).check();
    await expect(increase.getByRole('checkbox', { name: 'Sabedoria' })).toBeDisabled();
  });

  test('Elfo da Floresta deve mostrar o deslocamento da sub-raça', async ({ page, api }) => {
    const id = await api.create('Floresta', { raceId: 'elfo', subraceId: 'elfo-da-floresta' });
    await page.goto(`/fichas/${id}/raca`);

    // Pés Ligeiros: 9 m + 1,5 m (LdJ p. 24).
    await expect(page.getByText(/Deslocamento: 10,5 m/)).toBeVisible();
  });

  test('trocar de raça deve limpar escolhas e avisar', async ({ page, api }) => {
    const id = await api.create('Troca', { raceId: 'anao', subraceId: 'anao-da-colina' });
    await page.goto(`/fichas/${id}/raca`);
    await page.getByRole('radio', { name: /^Elfo / }).check();

    await expect(page.getByRole('status').filter({ hasText: 'Você trocou de raça' })).toBeVisible();
    await expect(page.getByRole('radio', { name: /^Anão da Colina/ })).toHaveCount(0);
  });
});
