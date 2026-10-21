# Spec F12: Arquétipos de classe

Pós-MVP (Fase 9, bloco C). Escopo: [ADR-0011](../../../docs/adr/ADR-0011-archetype-scope.md) — os 41 do LdJ,
em fatias por classe. Hoje o arquétipo é só o marco "escolha no nível N" com o selo "anote à mão"
(F9 AC3) e rótulos genéricos "Característica de <arquétipo>". **Muda o contrato público** (seção
"Contrato proposto"): exige aprovação humana antes de implementar.

## Objective
O iniciante escolhe o arquétipo da classe no nível certo e passa a ver, na ficha e no "Próximo
nível", as características reais dele por nível (com tipo e página), sem anotar nada à mão.

## User stories
- Como jogador de Ladino nv. 3, quero escolher "Ladrão" e ver o que ele me dá, não um rótulo genérico.
- Como iniciante, quero saber em que nível escolho e o que cada opção faz, antes de decidir.

## Acceptance criteria
1. **Passo Classe**: com classe e nível ≥ nível de escolha (1: Bruxo, Clérigo, Feiticeiro; 2: Druida,
   Mago; 3: demais — `subclassLevel`), aparece a seção "<Arquétipo de Ladino>" com um cartão por
   opção (nome + resumo de 1 frase); escolher grava `draft.archetype`. Abaixo desse nível, a seção
   diz "Você escolhe no nível N" (sem opções).
2. **Ficha**: com arquétipo escolhido, `features[]` traz as características dele até o nível, em
   ordem de nível (depois das da classe no mesmo nível), cada uma com `archetype` = nome, tipo,
   descrição curta e página; os rótulos genéricos "Característica de …" **somem**. Ex.: Guerreiro
   nv. 7 Campeão (p. 85): Crítico Aprimorado (nv. 3) e Atletismo Extraordinário (nv. 7); nv. 7
   sem arquétipo continua com o rótulo genérico do nv. 7.
3. **Próximo nível** inclui as características do arquétipo no nível N+1 (oculto no 20).
4. **Selo e aviso**: escolhido → a característica-marco deixa de ser `manual` e o selo "anote à
   mão" some. Não escolhido, nível ≥ marco → continua `manual` e o `warning` vira
   "Escolha seu <Arquétipo de Ladino> (nível 3)."
5. **Efeitos que o app já calcula** entram na conta: o arquétipo pode ter `bonuses[]` e `choices[]`
   (mesmo vocabulário das raças). Ex.: Linhagem Dracônica (Feiticeiro, p. 80) "Resiliência
   Dracônica" = +1 PV por nível → Feiticeiro nv. 3, Con 14 (+2): PV 23 = 6 + 6 + 8 + 3. Efeitos
   fora do vocabulário (CA, magias, ataques extras, críticos) ficam só como texto, como as
   características da classe base de hoje.
6. **"Meus bônus"** ganha o grupo "Arquétipo — <nome>" (bônus e escolhas); a conta do PV mostra a
   parcela com origem Arquétipo e página.
7. `400` em `errors.archetype`: id que não é arquétipo da classe do rascunho; sem `classId`; nível
   abaixo do marco; mais de um valor.
8. Trocar de classe limpa `archetype` e suas escolhas; baixar o nível abaixo do marco também, na
   mesma mudança, com aviso (como F8 AC2/F10 AC7). Trocar de arquétipo limpa só as escolhas dele.
9. Catálogo (T15/T16): cada arquétipo tem `srd`, ≥ 1 característica no nível do marco e só níveis
   que a classe reserva ao arquétipo; toda característica cita página do LdJ (ADR-0011 item 3).

## Contrato proposto (APROVAR)
- **Rascunho/arquivo**: `draft.archetype: string | null` (id, ex. `"ladrao"`). Aditivo; fichas
  antigas carregam sem ele; `schemaVersion` segue 1 (como F10). Escolhas dele usam `draft.choices`
  com ids prefixados pelo arquétipo (`ladrao-…`).
- **`GET /classes`**: cada classe ganha `archetypeLevel` e `archetypes[]`:
  `{ id, name, summary, srd, features: [{ id, name, level, kind, description, source }], choices: [...] }`
  (`choices` = escolhas dele, mesmo formato das raças, para a UI montar o seletor — acrescentado na T14).
- **`GET /classes/{id}/features?level=N&archetype=<id>`**: `archetype` opcional (prévia no passo
  Classe); inválido → `400` ProblemDetails.
- **Saída da ficha** (`/compute`, `GET /character-sheets/{id}`): `archetype: { id, name } | null`;
  `features[].archetype: string | null`; `bonuses[].origin` aceita o novo valor `"archetype"`.
- **Conteúdo** (não é API): `content/archetypes/<id>.json` com `classId` + schema novo; grant novo
  `archetype-slot` nos rótulos genéricos da classe (Domain os oculta quando há arquétipo).
- **Sem mudança**: rotas, códigos de erro existentes e `gen-api.sh` continua obrigatório (DTOs mudam).

## Decisões
- **Um arquivo por arquétipo** (regra de `ruleset-content.md`): fatias por classe sem inflar os 12
  JSON de classe; `classId` liga ao dono.
- **Selo fora do arquétipo**: as características dele são informativas, como as da classe base;
  só a escolha pendente gera aviso.
- **Sem magias** (ADR-0011 item 4): magias de domínio/juramento/círculo ficam no resumo.
- Escolha livre de trocar a qualquer momento (rascunho), sem custo — não há regra de "respec".

## Testes exigidos
- `Ruleset.Tests`: schema do arquétipo; `classId` existente; ≥ 1 característica no marco; níveis ⊆
  níveis reservados pela classe; `source` com página em tudo; `srd` presente; 41 ao fim da T16.
- `Domain.Tests`: AC2–AC5 (Campeão nv. 7; PV 23 do Feiticeiro dracônico; marco `manual` conforme
  escolha), AC7–AC8, invariante `Total == Σ Parts` com a parcela de PV do arquétipo.
- `Api.Tests`: `archetype` no `/compute` e no `PUT`; `errors.archetype`; `?archetype=` e `400`;
  `OpenApiSnapshotTests` + `scripts/gen-api.sh`.
- Vitest: seção do AC1 (abaixo e acima do marco); troca de classe/nível (AC8); grupo do AC6.
- E2E: Ladino nv. 3 escolhe Ladrão e vê as características na ficha; baixar para nv. 2 limpa e avisa; axe.

## Out of scope
Magias e listas de magias de arquétipo; CA calculada (Resiliência Dracônica, Defesa sem Armadura);
talentos e multiclasse; arquétipos de outros livros; mostrar o selo `srd` na interface.
