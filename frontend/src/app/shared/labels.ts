/**
 * Rótulos de exibição (pt-BR, termos da ficha oficial — AD-007). Só nomes e textos de UI:
 * nenhum número ou regra de cálculo mora aqui (ADR-0002).
 */
export interface TypeLabel {
  label: string;
  icon: string;
  /** Explicação de 1 frase para iniciante (legenda — F4 AC6). */
  hint: string;
}

/** `bonusType` → rótulo, ícone e explicação (docs/domain/bonus-types.md). */
export const BONUS_TYPES: Readonly<Record<string, TypeLabel>> = {
  'base-score': {
    label: 'Valor base',
    icon: '🎲',
    hint: 'O número que você escolheu para o atributo antes de qualquer bônus.',
  },
  'ability-score-increase': {
    label: 'Aumento de atributo',
    icon: '⬆️',
    hint: 'Pontos extras que a raça (ou uma escolha) soma direto no valor do atributo.',
  },
  'ability-modifier': {
    label: 'Modificador de atributo',
    icon: '⚖️',
    hint: 'O quanto o atributo ajuda ou atrapalha nas jogadas; sai do valor do atributo.',
  },
  'proficiency-bonus': {
    label: 'Bônus de proficiência',
    icon: '⭐',
    hint: 'Bônus que cresce com o nível e entra em tudo em que você é treinado.',
  },
  proficiency: {
    label: 'Proficiência',
    icon: '✔️',
    hint: 'Você é treinado nisso, então soma o bônus de proficiência.',
  },
  expertise: {
    label: 'Especialização',
    icon: '✨',
    hint: 'Treino de especialista: soma o dobro do bônus de proficiência.',
  },
  'half-proficiency': {
    label: 'Versatilidade',
    icon: '½',
    hint: 'Sem treino, mas soma metade do bônus de proficiência (arredondado para baixo).',
  },
  'hit-points': {
    label: 'Pontos de vida extra',
    icon: '❤️',
    hint: 'Pontos de vida a mais concedidos por raça ou classe.',
  },
  speed: { label: 'Deslocamento', icon: '👣', hint: 'Quantos metros você anda no seu turno.' },
  'armor-class': {
    label: 'Classe de Armadura',
    icon: '🛡️',
    hint: 'O quão difícil é acertar você.',
  },
};

/** `kind` de traços e características (benefícios não numéricos). */
export const KINDS: Readonly<Record<string, TypeLabel>> = {
  sense: {
    label: 'Sentido',
    icon: '👁️',
    hint: 'Um jeito especial de perceber o mundo, como enxergar no escuro.',
  },
  resistance: {
    label: 'Resistência a dano',
    icon: '🧱',
    hint: 'Você sofre só metade do dano daquele tipo.',
  },
  advantage: { label: 'Vantagem', icon: '🎯', hint: 'Role 2d20 e fique com o maior resultado.' },
  proficiency: {
    label: 'Proficiência',
    icon: '✔️',
    hint: 'Você sabe usar certas armas, armaduras, ferramentas ou perícias.',
  },
  language: { label: 'Idioma', icon: '💬', hint: 'Idiomas que você fala, lê e escreve.' },
  spell: { label: 'Magia', icon: '🔮', hint: 'Magias ou conjuração vindas da raça ou da classe.' },
  other: {
    label: 'Outro',
    icon: '📌',
    hint: 'Uma regra especial que não é um número fixo na ficha.',
  },
};

export const ABILITY_NAMES: Readonly<Record<string, string>> = {
  forca: 'Força',
  destreza: 'Destreza',
  constituicao: 'Constituição',
  inteligencia: 'Inteligência',
  sabedoria: 'Sabedoria',
  carisma: 'Carisma',
};

export const SKILL_NAMES: Readonly<Record<string, string>> = {
  acrobacia: 'Acrobacia',
  arcanismo: 'Arcanismo',
  atletismo: 'Atletismo',
  atuacao: 'Atuação',
  enganacao: 'Enganação',
  furtividade: 'Furtividade',
  historia: 'História',
  intimidacao: 'Intimidação',
  intuicao: 'Intuição',
  investigacao: 'Investigação',
  'lidar-com-animais': 'Lidar com Animais',
  medicina: 'Medicina',
  natureza: 'Natureza',
  percepcao: 'Percepção',
  persuasao: 'Persuasão',
  prestidigitacao: 'Prestidigitação',
  religiao: 'Religião',
  sobrevivencia: 'Sobrevivência',
};

const unknown = (id: string): TypeLabel => ({ label: id, icon: '•', hint: '' });
export const bonusType = (id: string): TypeLabel => BONUS_TYPES[id] ?? unknown(id);
export const kind = (id: string): TypeLabel => KINDS[id] ?? unknown(id);

/** `ability:forca` → "Força", `skill:atletismo` → "Atletismo"; outros textos passam direto. */
export function optionLabel(option: string): string {
  const [prefix, id] = option.split(':');
  if (prefix === 'ability' && id) {
    return ABILITY_NAMES[id] ?? option;
  }
  if (prefix === 'skill' && id) {
    return SKILL_NAMES[id] ?? option;
  }
  return option;
}

export const signed = (n: number): string =>
  n >= 0 ? `+${n.toLocaleString('pt-BR')}` : n.toLocaleString('pt-BR');
