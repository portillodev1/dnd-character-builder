# Spec F4: Bônus explicados por origem e tipo

Critério de pronto **4**. Tipos: `docs/domain/bonus-types.md`. Atravessa F1–F3.

## Objective
O iniciante entende, de forma intuitiva, **todos os bônus** do personagem: de onde vêm
(raça, sub-raça, classe, nível, atributo, escolha) e **que tipo de bônus** são.

## User stories
- Como iniciante, quero clicar em qualquer número da ficha e ver a conta.
- Como iniciante, quero uma visão "Meus bônus" agrupada por origem.

## Acceptance criteria
1. Todo valor numérico da ficha (atributos, modificadores, proficiência, salvaguardas,
   perícias, passiva, deslocamento, PV) é **clicável e focável por teclado** e abre o breakdown.
2. O breakdown lista parcelas na ordem base → atributo → proficiência → extras; cada parcela
   mostra valor com sinal, rótulo, **tipo** (rótulo + ícone + cor) e página do LdJ.
3. A soma das parcelas é sempre igual ao total exibido (invariante testada no back-end).
4. A visão "Meus bônus" agrupa por origem: **Raça**, **Sub-raça**, **Classe (nível N)**,
   **Escolhas** — cada item com tipo e alvo (ex.: "+2 Constituição · Aumento de atributo · Anão").
5. Benefícios não numéricos (Visão no Escuro, resistências, vantagens, idiomas) aparecem em
   "Meus bônus" com o `kind` e explicação curta; "Vantagem" explica 2d20.
6. Uma legenda fixa explica cada tipo de bônus em 1 frase para iniciante.
7. Exemplo verificável: Anão da Colina, Constituição base 14 → Constituição **16** =
   14 (Valor base) + 2 (Aumento de atributo · Anão); modificador **+3**.

## API
Todo `DerivedValue` no payload de `/compute` tem `total` e `parts[]` com `bonusType` e `source`;
nenhum número solto (ver `docs/architecture/api-contract.md`).

## Testes exigidos
- `Domain.Tests`: propriedade `Total == ΣParts` para todo `DerivedValue` da ficha (casos
  aleatórios de raça/classe/nível).
- `Api.Tests`: teste de contrato percorre o JSON e falha se achar valor numérico de ficha sem `parts`.
- Vitest: `bonus-breakdown` renderiza tipo com texto (não só cor) e responde a Enter/Espaço.

## Out of scope
Bônus de equipamento, magias e talentos (fase pós-MVP).
