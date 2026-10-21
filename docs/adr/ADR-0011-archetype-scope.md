# ADR-0011: Escopo dos arquétipos — todos os 41 do Livro do Jogador

- **Status**: aceito (2026-10-21)
- **Data**: 2026-10-21
- **Decisores**: Gabriel Portillo

## Contexto
Hoje o catálogo só tem as características da classe base; o arquétipo aparece como marco
("escolha no nível N") com o selo "anote à mão" (F9). O SRD 5.1 (CC-BY-4.0) cobre **1 arquétipo
por classe (12)**; o LdJ traz **41**. O ADR-0004 (item 4) já admite arquétipos fora do SRD com
"nome + mecânica resumida", sujeito à aprovação do autor — que esta decisão dá, para este escopo.

## Decisão
1. Entram **todos os 41 arquétipos do LdJ**, em fases por classe (ver abaixo), não de uma vez.
2. Os 12 do SRD 5.1 levam `srd: true` e valem as regras de licença do ADR-0004; os 29 restantes
   levam `srd: false`.
3. Conteúdo fora do SRD: **só nome, nível, tipo e resumo próprio** (≤ 3 frases, linguagem para
   iniciante) com a mecânica em números/regra, **nunca transcrição** do livro. Toda característica
   cita a página do LdJ (`source`), como o resto do catálogo.
4. **Fora do escopo** deste ADR: magias (as de domínio/juramento/círculo viram texto do resumo, não
   lista de magias — backlog "Magias"), talentos e multiclasse.

## Lista (LdJ, Cap. 3) e nível de escolha
| Classe | Nível | Arquétipos (SRD em **negrito**) |
|---|---|---|
| Bárbaro | 3 | **Caminho do Furioso**, Caminho do Guerreiro Totêmico |
| Bardo | 3 | **Colégio do Conhecimento**, Colégio da Bravura |
| Bruxo | 1 | Arquifada, **Corruptor**, Grande Antigo |
| Clérigo | 1 | Conhecimento, Enganação, Guerra, Luz, Natureza, Tempestade, **Vida** |
| Druida | 2 | **Círculo da Terra**, Círculo da Lua |
| Feiticeiro | 1 | **Linhagem Dracônica**, Magia Selvagem |
| Guerreiro | 3 | **Campeão**, Cavaleiro Arcano, Mestre de Batalha |
| Ladino | 3 | **Ladrão**, Assassino, Trapaceiro Arcano |
| Mago | 2 | Abjuração, Adivinhação, Conjuração, Encantamento, **Evocação**, Ilusão, Necromancia, Transmutação |
| Monge | 3 | **Caminho da Mão Aberta**, Caminho Sombrio, Caminho dos Quatro Elementos |
| Paladino | 3 | **Juramento de Devoção**, Juramento dos Anciões, Juramento de Vingança |
| Patrulheiro | 3 | Conclave da Besta, **Conclave do Caçador**, Conclave do Rastreador Subterrâneo |

## Alternativas consideradas
- **Só os 12 do SRD** — mais seguro para publicar, mas deixa a maioria dos jogadores (Totêmico,
  Mestre de Batalha, domínios do Clérigo…) com o selo "anote à mão".
- **SRD primeiro e o resto numa fase seguinte** — era a recomendação; o autor preferiu o escopo
  completo. Mantemos a entrega em fatias por classe (abaixo) para reduzir esse risco.

## Consequências
- **Positivas**: a ficha cobre o livro inteiro; o selo "anote à mão" do arquétipo some.
- **Negativas / trade-offs**: ~41 × 3–5 características para redigir e revisar; mais conteúdo
  `srd: false` para decidir antes de publicar o repositório.
- **Follow-ups**: (a) spec `class-archetypes` (T12): contrato `draft.archetype`, escolha no nível
  certo, características por nível no breakdown; (b) fatias de dados **por classe**, começando
  pelos 12 do SRD, para cada fatia passar no `verify.sh --strict`; (c) revisar a licença dos 29
  não-SRD antes de publicar; (d) magias de domínio/juramento seguem no backlog de Magias.
