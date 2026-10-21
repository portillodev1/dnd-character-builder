import { TestBed } from '@angular/core/testing';
import { BonusBreakdownComponent } from './bonus-breakdown';

describe('BonusBreakdownComponent', () => {
  async function render() {
    const fixture = TestBed.createComponent(BonusBreakdownComponent);
    fixture.componentRef.setInput('label', 'Constituição');
    fixture.componentRef.setInput('total', 16);
    fixture.componentRef.setInput('parts', [
      {
        value: 2,
        label: 'Anão',
        bonusType: 'ability-score-increase',
        source: { book: 'LdJ', page: 20 },
      },
      {
        value: 14,
        label: 'Valor base',
        bonusType: 'base-score',
        source: { book: 'LdJ', page: 13 },
      },
    ]);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    return { fixture, el, button: el.querySelector('button') as HTMLButtonElement };
  }

  it('breakdown quando aberto deve mostrar o tipo em texto e a página, base antes do aumento (F4 AC2)', async () => {
    const { fixture, el, button } = await render();

    button.click();
    await fixture.whenStable();

    const items = Array.from(el.querySelectorAll('li'), (li) =>
      Array.from(li.querySelectorAll('span'), (s) => s.textContent?.trim()).join(' | '),
    );
    expect(items).toEqual([
      '+14 | Valor base | ⚀ Valor base | LdJ p. 13',
      '+2 | Anão | ↑ Aumento de atributo | LdJ p. 20',
    ]);
  });

  it('breakdown com unidade deve mostrar o total decimal em pt-BR com a unidade (deslocamento)', async () => {
    const fixture = TestBed.createComponent(BonusBreakdownComponent);
    fixture.componentRef.setInput('label', 'Deslocamento');
    fixture.componentRef.setInput('total', 10.5);
    fixture.componentRef.setInput('unit', 'm');
    fixture.componentRef.setInput('parts', [
      { value: 9, label: 'Elfo', bonusType: 'speed', source: { book: 'LdJ', page: 23 } },
      { value: 1.5, label: 'Pés Ligeiros', bonusType: 'speed', source: { book: 'LdJ', page: 24 } },
    ]);
    await fixture.whenStable();
    const button = (fixture.nativeElement as HTMLElement).querySelector('button')!;

    button.click();
    await fixture.whenStable();

    const values = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('li .value'),
      (v) => v.textContent?.trim(),
    );
    expect([button.textContent?.trim(), ...values]).toEqual(['10,5 m', '+9', '+1,5']);
  });

  it('breakdown do deslocamento com bônus de classe deve listar a condição no rótulo e a página (F11 AC4)', async () => {
    const fixture = TestBed.createComponent(BonusBreakdownComponent);
    fixture.componentRef.setInput('label', 'Deslocamento');
    fixture.componentRef.setInput('total', 13.5);
    fixture.componentRef.setInput('unit', 'm');
    fixture.componentRef.setInput('parts', [
      { value: 9, label: 'Humano', bonusType: 'speed', source: { book: 'LdJ', page: 31 } },
      {
        value: 4.5,
        label: 'Movimento sem Armadura (nível 6) — sem armadura nem escudo',
        bonusType: 'speed',
        source: { book: 'LdJ', page: 104 },
      },
    ]);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;

    el.querySelector('button')!.click();
    await fixture.whenStable();

    expect(
      Array.from(el.querySelectorAll('li'), (li) =>
        Array.from(li.querySelectorAll('span'), (s) => s.textContent?.trim()).join(' | '),
      ),
    ).toEqual([
      '+9 | Humano | » Deslocamento | LdJ p. 31',
      '+4,5 | Movimento sem Armadura (nível 6) — sem armadura nem escudo | » Deslocamento | LdJ p. 104',
    ]);
  });

  it('breakdown aberto quando recebe Esc deve fechar e devolver o foco ao número (F4 AC1)', async () => {
    const { fixture, el, button } = await render();
    button.click();
    await fixture.whenStable();

    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    await fixture.whenStable();

    expect([el.querySelector('[role=region]'), button.getAttribute('aria-expanded')]).toEqual([
      null,
      'false',
    ]);
  });

  it('breakdown aberto quando o clique é fora deve fechar; dentro do painel, não', async () => {
    const { fixture, el, button } = await render();
    button.click();
    await fixture.whenStable();

    (el.querySelector('[role=region]') as HTMLElement).click();
    await fixture.whenStable();
    const aposClicarDentro = el.querySelector('[role=region]') !== null;
    document.body.click();
    await fixture.whenStable();

    expect([aposClicarDentro, el.querySelector('[role=region]')]).toEqual([true, null]);
  });

  it('breakdown quando outro número abre deve fechar o anterior (um painel por vez)', async () => {
    const primeiro = await render();
    const segundo = await render();
    primeiro.button.click();
    await primeiro.fixture.whenStable();

    segundo.button.click();
    await segundo.fixture.whenStable();
    await primeiro.fixture.whenStable();

    expect([
      primeiro.button.getAttribute('aria-expanded'),
      segundo.button.getAttribute('aria-expanded'),
    ]).toEqual(['false', 'true']);
  });

  it.each(['Enter', ' '])(
    'breakdown quando recebe a tecla "%s" deve alternar o painel (F4 AC1)',
    async (key) => {
      const { fixture, el, button } = await render();

      button.dispatchEvent(new KeyboardEvent('keydown', { key, bubbles: true }));
      await fixture.whenStable();

      expect(button.getAttribute('aria-expanded')).toBe('true');
      expect(el.querySelector('[role=region]')).not.toBeNull();
    },
  );
});
