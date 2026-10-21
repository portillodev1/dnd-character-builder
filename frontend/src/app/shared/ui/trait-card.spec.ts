import { TestBed } from '@angular/core/testing';
import { TraitCardComponent } from './trait-card';

describe('TraitCardComponent', () => {
  async function render(manual: boolean) {
    const fixture = TestBed.createComponent(TraitCardComponent);
    fixture.componentRef.setInput('name', 'Incremento no Valor de Habilidade');
    fixture.componentRef.setInput('kind', 'other');
    fixture.componentRef.setInput('description', 'd');
    fixture.componentRef.setInput('page', 91);
    fixture.componentRef.setInput('manual', manual);
    await fixture.whenStable();
    return (
      (fixture.nativeElement as HTMLElement).querySelector('.manual')?.textContent?.trim() ?? null
    );
  }

  it('característica que o app não aplica deve ter o selo "anote à mão" (F9 AC3)', async () => {
    expect([await render(true), await render(false)]).toEqual([
      'Ainda não aplicado nesta ficha — anote à mão.',
      null,
    ]);
  });
});
