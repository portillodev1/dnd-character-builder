# ADR-0008: Fontes de época empacotadas com @fontsource

- **Status**: aceito
- **Data**: 2026-10-20
- **Decisores**: Gabriel Portillo

## Contexto
O visual do front passou a seguir a estética de manuscrito medieval (tinta, rubricas em
vermelho, capitulares). Só com fontes do sistema a tipografia fica genérica — o macOS, por
exemplo, não tem nenhuma tipografia de época legível. Precisamos de fontes próprias sem
depender de serviço de terceiros em runtime (o app roda local, sem login, ADR-0002).

## Decisão
Vamos usar como `dependencies` exatas do `frontend/` (todas OFL 1.1):
- **@fontsource/eb-garamond 5.3.0** — texto corrido (400, 400 itálico, 600);
- **@fontsource/im-fell-english 5.3.0** — títulos (400 e itálico);
- **@fontsource/im-fell-english-sc 5.3.0** — versaletes de legendas, botões e passos.

Só o subconjunto `latin` (cobre pt-BR) entra no build, via `styles` do `angular.json`. As
famílias ficam nos tokens `--font-body`, `--font-display` e `--font-sc` de `src/styles.scss`,
com fallback serifado do sistema.

## Alternativas consideradas
- Google Fonts por `<link>` — sem pacote npm, mas o app chamaria um terceiro ao abrir e não
  funcionaria offline.
- Só fontes do sistema — sem dependência, mas a estética medieval fica só em cor e moldura.

## Consequências
- **Positivas**: tipografia de época idêntica em qualquer sistema; offline; sem chamadas externas.
- **Negativas / trade-offs**: ~100 kB de woff2 no build; três pacotes a atualizar.
- **Follow-ups**: nenhum.
