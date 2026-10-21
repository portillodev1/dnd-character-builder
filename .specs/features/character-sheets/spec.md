# Spec F5: Salvar, listar e editar fichas (arquivos JSON)

Critério de pronto **5**. Decisão: ADR-0006 (substitui a persistência do ADR-0002).
Contrato: `docs/architecture/api-contract-sheets.md`.

## Objective
O jogador guarda as fichas que montou, volta a elas depois e continua de onde parou.
Cada ficha é um arquivo JSON no servidor; toda alteração no wizard atualiza esse arquivo.

## User stories
- Como jogador, quero ver a lista das minhas fichas para escolher qual abrir.
- Como jogador, quero que minhas mudanças sejam salvas sem precisar lembrar de salvar.
- Como jogador, quero apagar uma ficha que não uso mais.

## Acceptance criteria
1. **Dado** a pasta de fichas, **quando** abro "Minhas fichas", **então** vejo cada ficha com
   nome, raça, classe, nível e data da última alteração, da mais recente para a mais antiga.
2. **Quando** clico em "Nova ficha" e dou um nome, **então** um arquivo `<id>.json` é criado e
   o wizard abre nessa ficha no passo Raça.
3. **Dado** uma ficha aberta, **quando** mudo qualquer escolha, **então** o arquivo é
   atualizado (salvamento automático com debounce) e a tela mostra "Salvo às HH:MM".
4. **Quando** abro uma ficha existente, **então** o wizard restaura todas as escolhas e a
   ficha derivada é **recalculada** pelas regras atuais (o arquivo guarda só escolhas).
5. Ficha incompleta pode ser salva; as escolhas pendentes aparecem como `warnings`.
6. **Dado** duas abas editando a mesma ficha, **quando** a aba desatualizada salva,
   **então** recebe `409` e a tela oferece recarregar (nada é sobrescrito em silêncio).
7. **Quando** apago uma ficha, **então** o front pede confirmação e o arquivo é removido;
   a ficha some da lista.
8. Renomear a ficha altera só o `name` no mesmo arquivo (o `id` não muda).

## Formato do arquivo (`data/fichas/<id>.json`)
```json
{ "schemaVersion": 1, "id": "0199f6c2-…", "name": "Thorin", "version": 3,
  "createdAt": "2026-10-20T18:00:00Z", "updatedAt": "2026-10-20T18:42:10Z",
  "draft": { "raceId": "anao", "subraceId": "anao-da-colina", "classId": "ladino",
             "level": 3, "baseAbilities": { "...": 0 }, "skillChoices": [], "expertise": [] } }
```

## API
`GET/POST /api/v1/character-sheets` · `GET/PUT/DELETE /api/v1/character-sheets/{id}`.
`404` para `id` inexistente; `400` para rascunho inválido; `409` para `version` divergente.

## Testes exigidos
- `Api.Tests`: CRUD completo com pasta temporária; `PUT` com `version` antiga → 409;
  `id` que não é GUID → 404 (sem tocar o disco); `GET` recalcula a ficha a partir do `draft`.
- `Infrastructure` (em `Api.Tests` ou projeto próprio): gravação atômica; arquivo corrompido
  → ficha listada com erro, sem derrubar a listagem.
- Vitest: lista ordenada por data; indicador "Salvo às"; diálogo de 409 e de exclusão.

## Out of scope
Login, multiusuário, compartilhar por link, exportar PDF, histórico de versões.
