# Spec F1: Traços e particularidades da raça

Critério de pronto **1**. Fonte: LdJ Cap. 2 (p. 17–44). Modelo: `docs/architecture/ruleset-data.md`.

## Objective
Ao escolher raça (e sub-raça, se houver), o iniciante vê **todas** as particularidades
dela, explicadas em linguagem simples: aumento de atributos, tamanho, deslocamento,
sentidos, resistências, proficiências, idiomas e traços especiais.

## User stories
- Como iniciante, quero comparar raças antes de escolher, para entender o que cada uma oferece.
- Como iniciante, quero ver o que a sub-raça adiciona à raça-mãe, sem misturar as duas.

## Acceptance criteria
1. **Dado** o catálogo, **quando** abro o passo Raça, **então** vejo as 9 raças do LdJ com
   resumo de 1 frase e principais aumentos de atributo.
2. **Dado** que escolho "Anão", **então** vejo sub-raças disponíveis e sou obrigado a escolher
   uma antes de avançar (raças sem sub-raça avançam direto).
3. **Dado** Anão + Anão da Colina, **então** vejo traços agrupados por origem:
   "Anão" (ex.: Visão no Escuro, Resiliência Anã) e "Anão da Colina" (ex.: Tenacidade Anã).
4. Cada traço mostra: nome, **tipo** (`kind`, ver `docs/domain/bonus-types.md`),
   descrição curta própria e página do LdJ.
5. Aumentos de atributo aparecem como bônus tipados (`ability-score-increase`) com valor e atributo.
6. Traços que exigem escolha (ex.: Meio-Elfo: +1 em dois atributos à escolha; Versatilidade
   em Perícia) mostram a escolha pendente e bloqueiam o avanço até serem feitos.
7. Trocar de raça limpa escolhas dependentes da raça anterior e avisa o usuário.

## API
`GET /api/v1/races`, `GET /api/v1/races/{id}` (ver `docs/architecture/api-contract.md`).
`404` ProblemDetails para `id` inexistente.

## Testes exigidos
- `Ruleset.Tests`: 9 raças válidas no schema; toda sub-raça referenciada existe; todo traço tem `source`.
- `Api.Tests`: lista com 9 itens; detalhe do Anão contém Visão no Escuro; 404 para `orc-xyz`.
- Vitest: componente agrupa traços por origem; bloqueia avanço com escolha pendente.

## Out of scope
Raças de outros livros; descrição narrativa longa (cultura, nomes).
