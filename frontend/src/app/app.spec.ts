import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes, wizardSteps } from './app.routes';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter(routes)],
    }).compileComponents();
  });

  it('app quando renderizado deve listar os 5 passos do wizard em ordem', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const links = (fixture.nativeElement as HTMLElement).querySelectorAll('nav a');

    expect(Array.from(links, (a) => a.textContent?.trim())).toEqual(
      wizardSteps.map((s) => s.label),
    );
  });
});
