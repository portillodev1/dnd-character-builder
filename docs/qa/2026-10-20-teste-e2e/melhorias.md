# Melhorias sugeridas (20/10/2026)

Voltar ao [resumo](README.md). Não são bugs; são ganhos de UX, robustez e cobertura.

## Ordem sugerida de correção
1. **FE-1** + **FE-2** — o usuário fica preso sem saber por quê (maior impacto no iniciante).
2. **BE-1** + **BE-2** + **BE-5** — robustez das fichas em arquivo (ADR-0006 convida a editá-las).
3. **BE-3** + **BE-4** + **FE-3** — 500 vira 400 com mensagem útil.
4. **FE-4**, **FE-5**, **FE-6** — erros de rede e acessibilidade dos diálogos.
5. Itens de baixa severidade.

## UX para iniciantes
- **Contador da compra de pontos**: mostrar "pontos gastos: 23 de 27" enquanto distribui. Hoje o
  jogador só descobre que estourou depois (e, com FE-1, nem descobre). O custo pode vir da API
  (`BaseScoreRules.PointBuyCost`) para não pôr regra no front (ADR-0002).
- **Nível ≥ 4 mostra "Incremento no Valor de Habilidade"** mas o wizard não aplica o aumento
  (ASI está no backlog do `ROADMAP.md`). Exibir um aviso "ainda não aplicado nesta ficha" para
  o iniciante não achar que os atributos estão errados. Idem "Arquétipo de Ladino" (nível 3).
- **Erros sempre visíveis**: um resumo de pendências/erros no topo do wizard com link para o
  passo que resolve (complementa FE-2).
- **Status do salvamento** com cor por estado (sucesso/erro) e botão "Tentar de novo" no estado
  `error` — hoje só uma nova mudança dispara outra tentativa.
- **Duplicar ficha** pelo app, evitando que o usuário copie arquivos à mão (ver BE-5).

## Testes (harness)
- `Api.Tests`: corpo malformado, tipos errados, `level` ausente, `choices` com `null`, arquivo sem
  `draft`, arquivo com id divergente — todos devem dar 4xx, nunca 500.
- Teste de contrato do front: usar uma resposta 400 **real** (gerada pela API) no
  `character-draft.store.spec.ts`; hoje o mock diverge do contrato.
- E2E de estados inválidos: compra de pontos estourada, baixar nível com Especializações, API fora.
- E2E de teclado nos diálogos (foco inicial, Esc, Tab preso) — o axe não cobre isso.
- Considerar um teste "nenhum 500" percorrendo o OpenAPI com entradas aleatórias (fuzz leve).

## Back-end
- Handler global que converte `BadHttpRequestException` em 400 ProblemDetails (BE-3).
- `ReadAsync` validando invariantes do arquivo (BE-1/BE-2/BE-5) e logando o caminho do arquivo
  problemático (hoje nenhum log de aplicação é emitido).
- `MapOpenApi()` exposto em todos os ambientes; restringir a Development se for publicar.
- Mensagens de validação com nomes legíveis em vez de ids (`skill:…`, `ability:…`).

## Front-end
- `API_BASE_URL` via config de ambiente (ou `proxy.conf.json` no `ng serve`), eliminando a URL fixa
  e a reescrita de rotas no E2E (FE-12).
- `<dialog>` nativo para confirmações (FE-6).
- Cancelar/ignorar respostas antigas de `classFeatures` (ex.: `resource()`/`httpResource` do
  Angular ou `switchMap`) (FE-11).
- `beforeunload` enquanto `status` for `pending`/`saving` (FE-13).

## Lacunas de escopo já conhecidas (ROADMAP.md)
Antecedentes completos, subclasses, ASI/talentos, magias, equipamento e CA, deslocamento de
classe (Monge, Bárbaro). Não contadas como bugs neste relatório.
