import { CharacterSheet, DerivedValue, Feature, Race, SavedSheet } from '../core/api/models';

/** Dados de teste no formato da API (tipos gerados) — sem regra de cálculo. */
export const value = (total: number): DerivedValue => ({
  total,
  parts: [
    {
      value: total,
      label: 'Valor base',
      bonusType: 'base-score',
      source: { book: 'LdJ', page: 13 },
    },
  ],
});

export function sheet(overrides: Partial<CharacterSheet> = {}): CharacterSheet {
  return {
    abilities: {},
    pointBuy: null,
    proficiencyBonus: value(2),
    savingThrows: {},
    skills: {},
    passivePerception: null,
    hitPoints: null,
    speed: null,
    features: [],
    nextLevelFeatures: [],
    traits: [],
    bonuses: [],
    warnings: [],
    ...overrides,
  };
}

export function feature(overrides: Partial<Feature> = {}): Feature {
  return {
    id: 'incremento-no-valor-de-habilidade-4',
    name: 'Incremento no Valor de Habilidade',
    level: 4,
    kind: 'other',
    description: 'd',
    scalingValue: null,
    manual: false,
    abilityImprovement: true,
    source: { book: 'LdJ', page: 85 },
    ...overrides,
  };
}

export function saved(overrides: Partial<SavedSheet> = {}): SavedSheet {
  return {
    id: '00000000-0000-0000-0000-000000000001',
    name: 'Thorin',
    version: 1,
    createdAt: '2026-10-20T18:00:00Z',
    updatedAt: '2026-10-20T18:00:00Z',
    draft: {},
    sheet: sheet(),
    errors: null,
    ...overrides,
  };
}

const src = (page: number) => ({ book: 'LdJ', page });

export const anao: Race = {
  id: 'anao',
  name: 'Anão',
  summary: 's',
  size: 'medio',
  speed: { meters: 7.5, note: null, label: null, source: src(20) },
  bonuses: [
    {
      target: 'ability:constituicao',
      value: 2,
      bonusType: 'ability-score-increase',
      label: 'Anão',
      source: src(20),
    },
  ],
  choices: [],
  traits: [
    {
      id: 'visao-no-escuro',
      name: 'Visão no Escuro',
      kind: 'sense',
      description: 'd',
      source: src(20),
    },
  ],
  languages: ['Comum', 'Anão'],
  subraces: [
    {
      id: 'anao-da-colina',
      name: 'Anão da Colina',
      summary: 's',
      srd: true,
      bonuses: [],
      choices: [],
      traits: [
        {
          id: 'tenacidade-ana',
          name: 'Tenacidade Anã',
          kind: 'other',
          description: 'd',
          source: src(20),
        },
      ],
      speed: null,
      source: src(20),
    },
  ],
  source: src(18),
};

export const meioElfo: Race = {
  ...anao,
  id: 'meio-elfo',
  name: 'Meio-Elfo',
  bonuses: [],
  traits: [],
  subraces: [],
  choices: [
    {
      id: 'aumento-de-atributo',
      name: 'Aumento de Atributo (Meio-Elfo)',
      count: 2,
      from: ['ability:forca', 'ability:destreza', 'ability:sabedoria'],
      kind: null,
      bonusType: 'ability-score-increase',
      source: src(39),
    },
  ],
};
