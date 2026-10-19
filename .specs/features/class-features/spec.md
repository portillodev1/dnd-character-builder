# Spec F2: Características da classe por nível

Critério de pronto **2**. Fonte: LdJ Cap. 3 (p. 45–122). Modelo: `docs/architecture/ruleset-data.md`.

## Objective
Ao escolher classe e nível (1–20), o iniciante vê **exatamente** as características
que o personagem já possui naquele nível, e o que vem a seguir.

## User stories
- Como iniciante, quero saber o que minha classe faz no nível em que vou começar.
- Como iniciante, quero ver o que ganho no próximo nível, para planejar.

## Acceptance criteria
1. **Dado** o catálogo, **quando** abro o passo Classe, **então** vejo as 12 classes com
   resumo, dado de vida, atributo principal e as 2 salvaguardas proficientes.
2. **Dado** Ladino nível 1, **então** vejo Especialização, Ataque Furtivo e Gíria de Ladrão —
   e **não** vejo Ação Ardilosa (nível 2).
3. **Dado** Ladino nível 3, **então** vejo também Ação Ardilosa e o marco
   "Arquétipo de Ladino — escolha no nível 3" (detalhe do arquétipo fora do MVP).
4. Características listadas em ordem de nível, cada uma com nível, tipo, descrição curta e página.
5. Valores que escalam por nível (ex.: dano do Ataque Furtivo, usos de Fúria) mostram o
   valor **do nível atual**, vindo da tabela da classe.
6. Uma seção "Próximo nível" mostra as características do nível N+1 (oculta no nível 20).
7. Proficiências da classe (armaduras, armas, ferramentas, salvaguardas, nº de perícias a
   escolher e lista) aparecem neste passo.
8. Mudar o nível atualiza a lista sem recarregar a página; nível fora de 1–20 é rejeitado.

## API
`GET /api/v1/classes`, `GET /api/v1/classes/{id}/features?level=N`.
`400` ProblemDetails para `level` inválido; `404` para classe inexistente.

## Testes exigidos
- `Domain.Tests`: filtro `level ≤ N` com casos 1, 2, 3, 20 por classe de exemplo.
- `Ruleset.Tests`: 12 classes; exatamente 2 salvaguardas; ≥ 1 feature no nível 1;
  colunas escalonáveis com 20 linhas.
- `Api.Tests`: Ladino nv. 1 sem Ação Ardilosa; nv. 3 com; `level=0` → 400.

## Out of scope
Magias por nível (lista de magias), multiclasse, detalhes de arquétipos.
