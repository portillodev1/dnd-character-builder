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
