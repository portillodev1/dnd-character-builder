# Spec F3: Perícias e salvaguardas — calcular e preencher

Critério de pronto **3**. Fórmulas: `docs/domain/calculations.md` §2–6. LdJ Cap. 7 (p. 174–181).

## Objective
O iniciante preenche as seções "Salvaguardas" e "Perícias" da ficha sabendo
**exatamente** como cada valor foi calculado e qual bolinha (proficiência) marcar.

## User stories
- Como iniciante, quero escolher minhas perícias de classe a partir da lista permitida.
- Como iniciante, quero ver a conta de cada perícia/salvaguarda, para conseguir refazer no papel.

## Acceptance criteria
1. **Dado** atributos definidos, **então** vejo as 6 salvaguardas com valor, bolinha marcada
   nas 2 da classe e a conta `mod + proficiência`.
2. **Dado** Ladino, **então** só posso escolher 4 perícias da lista do Ladino; o contador
   mostra "x de 4"; perícias fora da lista ficam desabilitadas com motivo.
3. Perícias já concedidas por raça (ex.: Elfo → Percepção) aparecem marcadas e travadas com
   a origem; escolher a mesma pela classe não é permitido.
4. As 18 perícias mostram atributo-chave (como na ficha: "Acrobacia (Des)"), valor final e
   conta. Exemplo verificável: Destreza 16, nível 1, Furtividade proficiente → **+5** = +3 +2.
5. Especialização (Ladino nv. 1/6, Bardo nv. 3/10): escolho perícias já proficientes e a conta
   mostra `+2 × proficiência`.
6. Versatilidade (Bardo nv. 2+): perícias sem proficiência somam `⌊prof/2⌋`.
7. Sabedoria Passiva (Percepção) = 10 + Percepção, com conta.
8. Um painel "Como preencher na ficha de papel" lista, por linha da ficha, o que escrever e
   qual bolinha marcar (gerado do resultado da API, não de regra no front).
9. "Antecedente": posso marcar 2 perícias extras (sem catálogo — assumption 5 do projeto).

## API
`POST /api/v1/character-sheets/compute` (seções `savingThrows`, `skills`, `passivePerception`).
Escolhas inválidas → `400`; escolhas incompletas → `warnings`.

## Testes exigidos
- `Domain.Tests` (tabela): modificador nos limites; proficiência por nível; perícia com fator
  0, ½, 1, 2; salvaguarda proficiente/não; passiva.
- `Api.Tests`: perícia fora da lista → 400; Especialização sem proficiência → 400.
- Vitest: contador de escolhas; perícia racial travada.

## Out of scope
Rolar dados; testes de habilidade situacionais; vantagem/desvantagem calculadas.
