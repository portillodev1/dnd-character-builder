# Spec F7: Fichas em arquivo à prova de edição manual

Pós-MVP (Fase 7). Estende F5 (`.specs/features/character-sheets/spec.md`) e ADR-0006.
Origem: relatório `docs/qa/2026-10-20-teste-e2e/` — BE-2, BE-5 e melhoria "duplicar ficha".
**Muda o contrato público** (AC1, AC2, AC5): exige aprovação humana antes de implementar.

## Objective
O ADR-0006 convida o jogador a abrir os arquivos num editor. Um arquivo editado errado ou
copiado à mão nunca causa `500`, nunca grava no arquivo de outra ficha e sempre explica o que
corrigir. Duplicar uma ficha passa a ser um botão, não uma cópia de arquivo.

## User stories
- Como jogador, quero saber qual arquivo está com problema e o que corrigir nele.
- Como jogador, quero duplicar um personagem para testar outra build sem perder o original.

## Acceptance criteria
1. **Dado** um arquivo com problema (JSON inválido, sem `name`/`draft`, `schemaVersion`
   desconhecido), **quando** `GET /character-sheets/{id}`, **então** `422` ProblemDetails
   `title: "Arquivo da ficha com problema"`, `detail` dizendo o que falta e o nome do arquivo.
2. **Dado** o mesmo arquivo, **quando** `PUT`, **então** `422` e o arquivo **não** é alterado;
   **quando** `DELETE`, **então** `204` (o app consegue limpar um arquivo quebrado).
3. **Dado** `<A>.json` cujo `id` interno é `B` (arquivo copiado/renomeado), **então** a lista
   mostra-o como arquivo com problema ("o id dentro do arquivo não bate com o nome do arquivo")
   e `GET`/`PUT` `/{A}` → `422`; nenhuma gravação acontece em `<B>.json`.
4. A lista nunca devolve dois itens com o mesmo `id` (consequência de AC3).
5. **Quando** clico em "Duplicar" numa ficha de "Minhas fichas", **então**
   `POST /character-sheets/{id}/duplicate` cria `<novo-id>.json` com o mesmo `draft`,
   `name` = "<nome> (cópia)" (truncado a 80), `version` 1, e o wizard abre a cópia no passo Raça.
6. Todo arquivo com problema encontrado gera um log `Warning` com o nome do arquivo e o motivo
   (uma vez por requisição; sem o conteúdo do arquivo; a pasta é a `Storage:SheetsPath`).

## Decisões
- O **nome do arquivo** é a identidade; o `id` interno é conferido, nunca usado para gravar.
- `422 Unprocessable Content` distingue "arquivo existe mas não dá para ler" de `404`.
- Novo `ResultStatus.Unreadable` na Application (mapeado para 422 em `Problems`).

## Testes exigidos
- `Api.Tests`: AC1–AC3 com arquivos escritos na pasta temporária (um caso por tipo de problema);
  AC2 confere o conteúdo do arquivo intacto após o `PUT`.
- `Api.Tests`: duplicar → `201`, `Location`, `draft` igual, nome com sufixo; id inexistente → `404`.
- Vitest: botão "Duplicar" chama a API e navega para a cópia.
- E2E (F5): duplicar uma ficha e ver as duas na lista.

## Out of scope
Reparar o arquivo automaticamente; histórico de versões; importar/exportar arquivos pelo app.
