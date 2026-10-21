# Revisão do catálogo × Livro do Jogador (T1, Fase 9)

Data: 2026-10-21 · Escopo: `content/races/*.json` (9) e `content/classes/*.json` (12).
Fonte: PDF local do LdJ em pt-BR (fora do repo, ADR-0004), extraído com `pdftotext`.
Método: conferência **automatizada** de números, níveis e nomes + leitura dirigida do texto.

## Resultado
**Nenhuma divergência numérica ou de regra encontrada.** Duas observações de nomenclatura
(sem ação obrigatória) e uma lista do que **não** foi coberto (abaixo).

## O que foi conferido (e bateu)
| Item | Como | Resultado |
|---|---|---|
| Dado de Vida, salvaguardas, nº de perícias e lista de perícias das 12 classes | texto da página da classe | 12/12 ok |
| Proficiências de armadura, armas e ferramentas das 12 classes | seção "Proficiências" | 12/12 ok (inclui Druida: sem metal) |
| Nível de cada característica de classe | linha do nível na tabela da classe | todas ok (só quebras de linha/nome ligeiramente diferente) |
| Colunas das tabelas (fúrias, dano, espaços de magia, truques, invocações, pacto, Movimento sem Armadura, Ki etc.) | linha do nível | 100% das colunas presentes no livro; ok |
| Colunas derivadas (ataques do Guerreiro, Crítico Brutal, dado de Inspiração, Canalizar Divindade, Forma Selvagem) | regra no texto da classe | coerentes com o livro |
| Aumento de atributo, tamanho, deslocamento e idiomas das 9 raças e sub-raças | seção da raça | 9/9 ok |
| Escolhas de raça (Meio-Elfo +1 ×2 e 2 perícias, Humano e idioma extra, Draconato ancestral) | seção da raça | ok |
| Sub-raças marcadas `srd: true` (Anão da Colina, Alto Elfo, Pés Leves, Gnomo das Rochas) | SRD 5.1 | ok |
| Páginas citadas em `source` (284 características/traços) | nome do item na página ±1 | 233 ok; 51 são rótulos genéricos (abaixo) |

## Observações (baixa gravidade)
1. **Nomes que o próprio livro escreve de dois jeitos**: "Canção **de**/**do** Descanso" (Bardo,
   título × tabela) e "Aura **de**/**da** Coragem" (Paladino). O catálogo usa o título da seção.
   Manter; só mudar se o revisor preferir o da tabela.
2. **51 itens "não achados" por nome** são rótulos próprios do catálogo, não texto do livro:
   "Característica de <arquétipo>" (placeholders; arquétipos estão fora do MVP), variantes
   "Ataque Extra (2)", "Indomável (dois usos)", "Arcana Mística (7º nível)",
   "Aprimoramento de …". As páginas citadas para eles (tabela da classe) estão corretas.
3. **Duergar**: o livro cita só como texto de ambientação, sem traços mecânicos; não é lacuna.

## Não coberto (revisão humana, se desejado)
- `description` (resumos em linguagem própria): fiz amostragem de leitura, não validação semântica
  de cada uma das ~284.
- Texto de escolhas (ex.: lista de Estilo de Luta, ancestrais dracônicos) além de contagem.
- Magias, equipamento e arquétipos (fora do MVP).

## Próximo passo
T2: revisão humana dos critérios F1–F5 no app. Nenhuma correção de catálogo (T3) é necessária.
