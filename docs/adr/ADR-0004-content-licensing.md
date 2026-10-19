# ADR-0004: Conteúdo baseado no SRD 5.1; PDF do livro não versionado

- **Status**: aceito (2026-10-19)
- **Data**: 2026-10-19
- **Decisores**: Gabriel Portillo

## Contexto
O Livro do Jogador é material protegido por direitos autorais. O **SRD 5.1** da Wizards
está sob **CC-BY-4.0** e cobre todas as raças e classes base, mas só **uma sub-raça por
raça e um arquétipo por classe**. O PDF de referência é uma tradução não oficial.

## Decisão
1. O PDF **nunca** entra no repositório; usado só para conferir regras e citar páginas.
2. `description` no catálogo é **resumo próprio**, curto, para iniciantes — nunca transcrição.
3. Números e mecânicas (bônus, níveis, salvaguardas) seguem o SRD 5.1 com atribuição
   CC-BY-4.0 no README; páginas citadas apontam para o LdJ para quem tem o livro.
4. Sub-raças/arquétipos fora do SRD: incluir só nome + mecânica resumida (aprovado pelo autor).

## Alternativas consideradas
- Transcrever o livro — risco legal; inviabiliza publicar o projeto.
- Só SRD estrito — perde sub-raças populares (ex.: Elfo da Floresta, Anão da Montanha).

## Consequências
- **Positivas**: projeto publicável; texto mais didático que o original.
- **Negativas**: esforço de redação dos resumos; decisão pendente sobre conteúdo não-SRD.
- **Follow-ups**: revisar resumos na Fase 3 antes de publicar o repositório.
