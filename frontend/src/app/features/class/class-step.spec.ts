import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ClassFeatures } from '../../core/api/models';
import { feature } from '../../testing/fixtures';
import { RulesetService } from '../../core/api/ruleset.service';
import { SheetService } from '../../core/api/sheet.service';
import { CharacterDraftStore } from '../../state/character-draft.store';
import { ClassStepComponent } from './class-step';

/** Só o que o passo lê; Especializações por nível vêm da API (F3 AC5). */
const rogue = (level: number, expertiseCount: number) =>
  ({
    id: 'ladino',
    name: 'Ladino',
    level,
    expertiseCount,
    hitDie: 8,
    savingThrows: [],
    skillChoices: { count: 4, from: [] },
    proficiencies: { armor: [], weapons: [], tools: [] },
    features: [],
    nextLevel: [],
  }) as unknown as ClassFeatures;

describe('ClassStepComponent', () => {
  const ruleset = { classes: vi.fn().mockResolvedValue([]), classFeatures: vi.fn() };

  async function render(draft: Parameters<CharacterDraftStore['draft']['set']>[0]) {
    TestBed.configureTestingModule({
      imports: [ClassStepComponent],
      providers: [
        provideRouter([]),
        { provide: RulesetService, useValue: ruleset },
        { provide: SheetService, useValue: {} },
      ],
    });
    const store = TestBed.inject(CharacterDraftStore);
    store.draft.set(draft);
    const fixture = TestBed.createComponent(ClassStepComponent);
    await fixture.whenStable();
    return { fixture, store, el: fixture.nativeElement as HTMLElement };
  }

  async function pickLevel(el: HTMLElement, level: number) {
    const select = el.querySelector('#level') as HTMLSelectElement;
    select.value = String(level);
    select.dispatchEvent(new Event('change'));
  }

  it('baixar o nível deve desmarcar as Especializações excedentes e avisar quais (F8 AC2)', async () => {
    ruleset.classFeatures.mockImplementation((_: string, level: number) =>
      Promise.resolve(rogue(level, level >= 6 ? 4 : 2)),
    );
    const { fixture, store, el } = await render({
      classId: 'ladino',
      level: 6,
      expertise: ['furtividade', 'percepcao', 'acrobacia', 'enganacao'],
    });

    await pickLevel(el, 3);
    await fixture.whenStable();

    expect([store.draft().level, store.draft().expertise]).toEqual([
      3,
      ['furtividade', 'percepcao'],
    ]);
    expect(el.querySelector('[role=status]')?.textContent?.trim()).toBe(
      'No nível 3 o Ladino concede 2 Especializações: desmarcamos Acrobacia e Enganação.',
    );
  });

  it('trocar o nível rápido deve mostrar as características do último nível, mesmo fora de ordem (F8 AC11)', async () => {
    const pending = new Map<number, (d: ClassFeatures) => void>();
    ruleset.classFeatures.mockImplementation(
      (_: string, level: number) => new Promise((resolve) => pending.set(level, resolve)),
    );
    const { fixture, el } = await render({ classId: 'ladino', level: 1 });

    await pickLevel(el, 2);
    await fixture.whenStable();
    await pickLevel(el, 3);
    await fixture.whenStable();
    pending.get(3)!(rogue(3, 2));
    await fixture.whenStable();
    pending.get(2)!(rogue(2, 2));
    await fixture.whenStable();

    expect(el.querySelector('#features')?.textContent?.trim()).toBe(
      'Características até o nível 3',
    );
  });

  it('baixar o nível deve remover as escolhas de Incrementos acima do novo nível e avisar (F10 AC7)', async () => {
    const asi = (level: number) =>
      feature({ id: `incremento-no-valor-de-habilidade-${level}`, level });
    ruleset.classFeatures.mockImplementation((_: string, level: number) =>
      Promise.resolve({
        ...rogue(level, 2),
        features: [4, 8].filter((l) => l <= level).map(asi),
      }),
    );
    const { fixture, store, el } = await render({
      classId: 'ladino',
      level: 8,
      abilityImprovements: {
        'incremento-no-valor-de-habilidade-4': ['forca', 'forca'],
        'incremento-no-valor-de-habilidade-8': ['destreza', 'carisma'],
      },
    });

    await pickLevel(el, 6);
    await fixture.whenStable();

    expect([
      store.draft().abilityImprovements,
      el.querySelector('[role=status]')?.textContent?.trim(),
    ]).toEqual([
      { 'incremento-no-valor-de-habilidade-4': ['forca', 'forca'] },
      'No nível 6 saem as escolhas de Incremento no Valor de Habilidade (nível 8).',
    ]);
  });

  it('trocar de classe deve limpar as escolhas de Incrementos (F10 AC7)', async () => {
    ruleset.classes.mockResolvedValueOnce([
      {
        id: 'ladino',
        name: 'Ladino',
        summary: 's',
        hitDie: 8,
        primaryAbilities: [],
        savingThrows: [],
      },
      {
        id: 'guerreiro',
        name: 'Guerreiro',
        summary: 's',
        hitDie: 10,
        primaryAbilities: [],
        savingThrows: [],
      },
    ]);
    ruleset.classFeatures.mockImplementation((_: string, level: number) =>
      Promise.resolve(rogue(level, 2)),
    );
    const { fixture, store, el } = await render({
      classId: 'ladino',
      level: 4,
      abilityImprovements: { 'incremento-no-valor-de-habilidade-4': ['forca', 'forca'] },
    });

    (el.querySelector('input[value=guerreiro]') as HTMLInputElement).dispatchEvent(
      new Event('change'),
    );
    await fixture.whenStable();

    expect(store.draft().abilityImprovements).toEqual({});
  });

  describe('arquétipo (spec F12)', () => {
    const src = { book: 'LdJ', page: 80 };
    const dragon = {
      id: 'linhagem-draconica-ancestral',
      name: 'Ancestral Dracônico',
      count: 1,
      from: ['Azul', 'Vermelho'],
      kind: 'other',
      bonusType: null,
      source: src,
    };
    const classes = [
      {
        id: 'guerreiro',
        name: 'Guerreiro',
        summary: 's',
        hitDie: 10,
        primaryAbilities: [],
        savingThrows: [],
        archetypeLevel: 3,
        archetypes: [
          {
            id: 'campeao',
            name: 'Campeão',
            summary: 'Golpes críticos mais frequentes.',
            srd: true,
            features: [feature({ id: 'critico-aprimorado', name: 'Crítico Aprimorado', level: 3 })],
            choices: [],
            source: src,
          },
        ],
      },
      {
        id: 'feiticeiro',
        name: 'Feiticeiro',
        summary: 's',
        hitDie: 6,
        primaryAbilities: [],
        savingThrows: [],
        archetypeLevel: 1,
        archetypes: [
          {
            id: 'linhagem-draconica',
            name: 'Linhagem Dracônica',
            summary: 'Sangue de dragão.',
            srd: true,
            features: [],
            choices: [dragon],
            source: src,
          },
        ],
      },
    ];
    const marker = (name: string, level: number) =>
      feature({ id: 'marco', name, level, manual: true, abilityImprovement: false });

    beforeEach(() => {
      ruleset.classes.mockResolvedValueOnce(classes);
      ruleset.classFeatures.mockImplementation((id: string, level: number) =>
        Promise.resolve({
          ...rogue(level, 0),
          name: id,
          features: [
            marker(
              id === 'guerreiro' ? 'Arquétipo Marcial' : 'Origem de Feitiçaria',
              id === 'guerreiro' ? 3 : 1,
            ),
          ],
        }),
      );
    });

    const options = (el: HTMLElement) =>
      Array.from(
        el.querySelectorAll('input[name=archetype]'),
        (i) => (i as HTMLInputElement).value,
      );

    it('abaixo do nível do marco deve explicar quando se escolhe, sem opções (AC1)', async () => {
      const { fixture, el } = await render({ classId: 'guerreiro', level: 2 });
      await fixture.whenStable();

      expect([options(el), el.querySelector('[role=note]')?.textContent?.trim()]).toEqual([
        [],
        'Você escolhe seu Arquétipo Marcial no nível 3.',
      ]);
    });

    it('no nível do marco deve listar as opções e gravar draft.archetype ao escolher (AC1)', async () => {
      const { fixture, store, el } = await render({ classId: 'guerreiro', level: 3 });
      await fixture.whenStable();
      const radio = el.querySelector('input[name=archetype][value=campeao]') as HTMLInputElement;

      radio.dispatchEvent(new Event('change'));
      await fixture.whenStable();

      expect([
        options(el),
        el.querySelector('fieldset.archetypes legend')?.textContent?.trim(),
        el.querySelector('fieldset.archetypes .meta')?.textContent?.trim(),
        store.draft().archetype,
        ruleset.classFeatures.mock.lastCall,
      ]).toEqual([
        ['campeao'],
        'Arquétipo Marcial',
        'Ganha: Crítico Aprimorado (nível 3)',
        'campeao',
        ['guerreiro', 3, 'campeao'],
      ]);
    });

    it('escolha do arquétipo deve aparecer como seletor e gravar em draft.choices', async () => {
      const { fixture, store, el } = await render({
        classId: 'feiticeiro',
        level: 1,
        archetype: 'linhagem-draconica',
      });
      await fixture.whenStable();

      (
        el.querySelector('app-choice-picker input[type=checkbox]') as HTMLInputElement
      ).dispatchEvent(new Event('change'));
      await fixture.whenStable();

      expect(store.draft().choices).toEqual({ 'linhagem-draconica-ancestral': ['Azul'] });
    });

    it('trocar de classe deve limpar o arquétipo e as escolhas dele (AC8)', async () => {
      const { fixture, store, el } = await render({
        classId: 'feiticeiro',
        level: 3,
        archetype: 'linhagem-draconica',
        choices: { 'linhagem-draconica-ancestral': ['Azul'], 'idioma-adicional': ['Anão'] },
      });
      await fixture.whenStable();

      (el.querySelector('input[value=guerreiro]') as HTMLInputElement).dispatchEvent(
        new Event('change'),
      );
      await fixture.whenStable();

      expect([store.draft().archetype, store.draft().choices]).toEqual([
        null,
        { 'idioma-adicional': ['Anão'] },
      ]);
    });

    it('baixar o nível abaixo do marco deve limpar o arquétipo e avisar (AC8)', async () => {
      const { fixture, store, el } = await render({
        classId: 'guerreiro',
        level: 3,
        archetype: 'campeao',
      });
      await fixture.whenStable();

      await pickLevel(el, 2);
      await fixture.whenStable();

      expect([store.draft().level, store.draft().archetype]).toEqual([2, null]);
      expect(el.querySelector('[role=status]')?.textContent?.trim()).toBe(
        'No nível 2 o Arquétipo Marcial ainda não existe (é escolhido no nível 3): tiramos Campeão.',
      );
    });

    it('baixar o nível sem passar do marco deve manter o arquétipo', async () => {
      const { fixture, store, el } = await render({
        classId: 'guerreiro',
        level: 7,
        archetype: 'campeao',
      });
      await fixture.whenStable();

      await pickLevel(el, 4);
      await fixture.whenStable();

      expect([store.draft().archetype, el.querySelector('[role=status]')]).toEqual([
        'campeao',
        null,
      ]);
    });
  });
});
