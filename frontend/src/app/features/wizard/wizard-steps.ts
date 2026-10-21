export interface WizardStep {
  path: string;
  label: string;
}

/** Ordem dos passos do wizard (docs/architecture/frontend.md). */
export const wizardSteps: readonly WizardStep[] = [
  { path: 'raca', label: 'Raça' },
  { path: 'classe', label: 'Classe' },
  { path: 'atributos', label: 'Atributos' },
  { path: 'pericias', label: 'Perícias e salvaguardas' },
  { path: 'ficha', label: 'Ficha' },
];

/** Caminho do passo seguinte/anterior, para os botões "Próximo"/"Voltar". */
export function neighbor(path: string, delta: 1 | -1): string | null {
  const i = wizardSteps.findIndex((s) => s.path === path);
  return wizardSteps[i + delta]?.path ?? null;
}

/** Campo do rascunho (raiz da chave de erro da API) → passo onde o jogador o corrige (F8 AC1). */
const FIELD_STEPS: Readonly<Record<string, string>> = {
  raceId: 'raca',
  subraceId: 'raca',
  choices: 'raca',
  classId: 'classe',
  level: 'classe',
  abilityMethod: 'atributos',
  baseAbilities: 'atributos',
  skillChoices: 'pericias',
  extraProficiencies: 'pericias',
  expertise: 'pericias',
};

/** `choices.aumento-de-atributo` → passo Raça; `null` para campos fora dos passos (ex.: `name`). */
export function stepOfField(field: string): WizardStep | null {
  const path = FIELD_STEPS[field.split(/[.[]/)[0]];
  return wizardSteps.find((s) => s.path === path) ?? null;
}
