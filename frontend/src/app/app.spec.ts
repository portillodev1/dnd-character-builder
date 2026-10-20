import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { wizardSteps } from './features/wizard/wizard-steps';

describe('App', () => {
  it('app quando renderizado deve ter o título com link para Minhas fichas', async () => {
    TestBed.configureTestingModule({ imports: [App], providers: [provideRouter([])] });
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();

    expect((fixture.nativeElement as HTMLElement).querySelector('h1 a')?.getAttribute('href')).toBe(
      '/fichas',
    );
  });

  it('wizard deve ter os 5 passos em ordem', () => {
    expect(wizardSteps.map((s) => s.label)).toEqual([
      'Raça',
      'Classe',
      'Atributos',
      'Perícias e salvaguardas',
      'Ficha',
    ]);
  });
});
