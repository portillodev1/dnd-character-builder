import { expectAccessible, expect, standardArray, test } from './support';

// Spec F11 — .specs/features/class-speed-bonus (Fase 9, bloco B).
// Humano (9 m, LdJ p. 31) Monge nv. 6: + 4,5 m de Movimento sem Armadura (p. 104) = 13,5 m.
test.describe('F11 Bônus de deslocamento da classe', () => {
  test('Monge nv. 6 deve mostrar 13,5 m e a parcela da classe na conta do Deslocamento', async ({
    page,
    api,
  }) => {
    const id = await api.create('Monge veloz', {
      raceId: 'humano',
      classId: 'monge',
      level: 6,
      ...standardArray([10, 15, 14, 13, 12, 8]),
    });
    await page.goto(`/fichas/${id}/ficha`);

    await page.getByRole('button', { name: 'Deslocamento: 13,5 m. Ver a conta' }).click();

    const conta = page.getByRole('region', { name: 'Conta de Deslocamento' });
    await expect(conta).toContainText('Movimento sem Armadura (nível 6) — sem armadura nem escudo');
    await expect(conta).toContainText('+4,5');
    await expect(conta).toContainText('LdJ p. 104');
    await expectAccessible(page);
  });

  test('Bárbaro deve ganhar o Movimento Rápido só no nível 5', async ({ page, api }) => {
    const speeds: string[] = [];
    for (const level of [4, 5]) {
      const id = await api.create(`Bárbaro ${level}`, {
        raceId: 'humano',
        classId: 'barbaro',
        level,
        ...standardArray([15, 14, 13, 12, 10, 8]),
      });
      await page.goto(`/fichas/${id}/ficha`);
      speeds.push(
        (
          await page
            .getByRole('button', { name: /^Deslocamento: .* m\. Ver a conta$/ })
            .textContent()
        )?.trim() ?? '',
      );
    }
    expect(speeds).toEqual(['9 m', '12 m']);
  });
});
