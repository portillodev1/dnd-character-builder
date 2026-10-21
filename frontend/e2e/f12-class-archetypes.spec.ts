import { expectAccessible, expect, standardArray, test } from './support';

// Spec F12 — .specs/features/class-archetypes (Fase 9, bloco C).
// Humano Guerreiro: Campeão (LdJ p. 85); Feiticeiro: Linhagem Dracônica (p. 80), +1 PV por nível.
const saved = (page: import('@playwright/test').Page) =>
  expect(page.getByRole('status').filter({ hasText: /Salvo às/ })).toBeVisible();

test.describe('F12 Arquétipos de classe', () => {
  test('Guerreiro nv. 7 deve escolher o Campeão e ver as características dele no lugar dos rótulos genéricos', async ({
    page,
    api,
  }) => {
    const id = await api.create('Aldric', {
      raceId: 'humano',
      classId: 'guerreiro',
      level: 7,
      ...standardArray([15, 14, 13, 12, 10, 8]),
    });
    await page.goto(`/fichas/${id}/classe`);

    const group = page.getByRole('group', { name: 'Arquétipo Marcial' });
    await group.getByRole('radio', { name: /Campeão/ }).check();
    await saved(page);

    await expect(page.getByRole('heading', { name: /Crítico Aprimorado/ })).toBeVisible();
    await expect(page.getByRole('heading', { name: /Atletismo Extraordinário/ })).toBeVisible();
    await expect(
      page.getByRole('heading', { name: /Característica de Arquétipo Marcial/ }),
    ).toHaveCount(0);
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(page.getByRole('heading', { name: 'Arquétipo — Campeão' })).toBeVisible();
    await expect(page.getByText('Crítico Aprimorado · Outro · nível 3')).toBeVisible();
    await expect(page.getByText(/Escolha seu Arquétipo Marcial/)).toHaveCount(0);
    await expectAccessible(page);
  });

  test('abaixo do marco a seção só explica; baixar o nível com arquétipo escolhido deve limpar e avisar', async ({
    page,
    api,
  }) => {
    const id = await api.create('Aldric', {
      raceId: 'humano',
      classId: 'guerreiro',
      level: 3,
      archetype: 'campeao',
      ...standardArray([15, 14, 13, 12, 10, 8]),
    });
    await page.goto(`/fichas/${id}/classe`);
    await expect(page.getByRole('radio', { name: /Campeão/ })).toBeChecked();

    await page.getByLabel('Nível do personagem').selectOption('2');

    await expect(page.getByText('tiramos Campeão')).toBeVisible();
    await expect(page.getByText('Você escolhe seu Arquétipo Marcial no nível 3.')).toBeVisible();
    await expect(page.getByRole('radio', { name: /Campeão/ })).toHaveCount(0);
    await saved(page);
  });

  test('Feiticeiro dracônico deve escolher o ancestral e ver a Resiliência Dracônica nos PV', async ({
    page,
    api,
  }) => {
    const id = await api.create('Ignis', {
      raceId: 'humano',
      classId: 'feiticeiro',
      level: 3,
      ...standardArray([8, 10, 14, 12, 13, 15]),
    });
    await page.goto(`/fichas/${id}/classe`);
    await page.getByRole('radio', { name: /Linhagem Dracônica/ }).check();
    await page
      .getByRole('group', { name: /Ancestral Dracônico/ })
      .getByLabel('Vermelho')
      .check();
    await saved(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(
      page.getByRole('heading', { name: 'Ancestral Dracônico: Vermelho' }),
    ).toBeVisible();
    await page.getByRole('button', { name: /^Pontos de Vida: .* Ver a conta$/ }).click();
    await expect(page.getByRole('region', { name: 'Conta de Pontos de Vida' })).toContainText(
      'Resiliência Dracônica',
    );
    await expectAccessible(page);
  });

  test('Clérigo deve ter os 7 domínios e o do Conhecimento deve pedir 2 idiomas e 2 perícias', async ({
    page,
    api,
  }) => {
    const id = await api.create('Sábia', {
      raceId: 'humano',
      classId: 'clerigo',
      level: 1,
      ...standardArray([10, 13, 14, 12, 15, 8]),
    });
    await page.goto(`/fichas/${id}/classe`);

    const group = page.getByRole('group', { name: 'Domínio Divino' });
    await expect(group.getByRole('radio')).toHaveCount(7);
    await group.getByRole('radio', { name: /Domínio do Conhecimento/ }).check();
    const idiomas = page.getByRole('group', { name: /Bênçãos do Conhecimento: idiomas/ });
    await idiomas.getByLabel('Anão').check();
    await idiomas.getByLabel('Élfico').check();
    const pericias = page.getByRole('group', { name: /Bênçãos do Conhecimento: perícias/ });
    await pericias.getByLabel('Arcanismo').check();
    await pericias.getByLabel('Religião').check();
    await saved(page);
    await expectAccessible(page);

    await page.goto(`/fichas/${id}/ficha`);
    await expect(page.getByText(/Escolha seu Domínio Divino/)).toHaveCount(0);
    await expect(page.getByText('+2 Arcanismo · Proficiência')).toBeVisible();
    await expect(
      page.getByRole('heading', { name: /^Bênçãos do Conhecimento · nível 1/ }),
    ).toBeVisible();
  });
});
