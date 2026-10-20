import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'fichas' },
  {
    path: 'fichas',
    title: 'Minhas fichas',
    loadComponent: () => import('./features/my-sheets/my-sheets').then((m) => m.MySheetsComponent),
  },
  {
    path: 'fichas/:id',
    loadComponent: () =>
      import('./features/wizard/wizard-shell').then((m) => m.WizardShellComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'raca' },
      {
        path: 'raca',
        title: 'Raça',
        loadComponent: () => import('./features/race/race-step').then((m) => m.RaceStepComponent),
      },
      {
        path: 'classe',
        title: 'Classe e nível',
        loadComponent: () =>
          import('./features/class/class-step').then((m) => m.ClassStepComponent),
      },
      {
        path: 'atributos',
        title: 'Atributos',
        loadComponent: () =>
          import('./features/abilities/abilities-step').then((m) => m.AbilitiesStepComponent),
      },
      {
        path: 'pericias',
        title: 'Perícias e salvaguardas',
        loadComponent: () =>
          import('./features/skills-saves/skills-saves-step').then(
            (m) => m.SkillsSavesStepComponent,
          ),
      },
      {
        path: 'ficha',
        title: 'Ficha',
        loadComponent: () =>
          import('./features/sheet/sheet-step').then((m) => m.SheetStepComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'fichas' },
];
