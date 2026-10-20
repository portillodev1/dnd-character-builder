# Back-end — bugs encontrados (20/10/2026)

Voltar ao [resumo](README.md). Reproduções contra a API isolada (`/api/v1`).

## BE-1 · Alta — um arquivo sem `draft` derruba a lista inteira
- **Reproduzir**: na pasta de fichas, um `<guid>.json` válido mas sem `"draft"` (ou `"draft": null`)
  → `GET /character-sheets` → **500**. "Minhas fichas" fica inutilizável para todas as fichas.
- **Causa**: `FileCharacterSheetStore.ListAsync` só captura `JsonException`/`InvalidDataException`;
  `s.Draft.RaceId` lança `NullReferenceException` fora do `catch`.
- **Correção**: em `ReadAsync`, validar o conteúdo após desserializar (`Draft`/`Name` nulos →
  `InvalidDataException`). Assim o arquivo entra na lista como "arquivo com problema" (spec F5).

## BE-2 · Média — abrir/salvar ficha com arquivo corrompido → 500
- `GET /character-sheets/{id}` e `PUT` de arquivo com JSON inválido ou sem `draft` → **500**.
- A lista orienta "abra-o num editor para corrigir", mas o front, ao abrir, mostra "Ficha não
  encontrada" (ver FE-5).
- **Correção**: mapear `InvalidDataException`/`JsonException` do store para um
  `Result` próprio → ProblemDetails 422 (ou 409) com mensagem para iniciante.

## BE-3 · Média — erro de binding vira 500 em vez de 400
| Requisição | Esperado | Obtido |
|---|---|---|
| `GET /classes/ladino/features` (sem `level`) | 400 | 500 |
| `GET /classes/ladino/features?level=abc` | 400 | 500 |
| `POST /compute` com `{bad json` ou sem corpo | 400 | 500 |
| `POST /compute` `{"level":"3"}` / `{"baseAbilities":{"forca":12.5}}` | 400 | 500 |
| `POST /compute` `{"baseAbilities":{"forca":null}}` / `level` > int32 | 400 | 500 |
- **Causa**: em Development `BadHttpRequestException` é lançada e o `UseExceptionHandler()`
  responde 500. Também afeta o front (FE-3).
- **Correção**: `app.UseExceptionHandler(new ExceptionHandlerOptions { StatusCodeSelector =
  ex => ex is BadHttpRequestException b ? b.StatusCode : 500 })` e/ou `int? level` com
  validação explícita. Adicionar casos em `Api.Tests`.

## BE-4 · Média — escolha racial com `null` → 500
- `POST /compute` `{"raceId":"meio-elfo","choices":{"versatilidade-em-pericia":null}}` → **500**
  (`picks.Count` em `ResolvedDraft.ResolveRaceChoices`).
- Relacionado: `{"skillChoices":[null]}` → 400 com mensagem "Perícia '' não existe."
- **Correção**: tratar lista nula como vazia no `ToDomain()` e itens nulos como inválidos.

## BE-5 · Média — id do conteúdo ≠ nome do arquivo
- **Cenário realista**: o usuário copia `A.json` para `B.json` para "duplicar" um personagem
  (ADR-0006 incentiva editar os arquivos).
- **Efeitos**: a lista devolve **duas entradas com o mesmo `id`**; `GET /B` devolve id A;
  `PUT /B` grava em `A.json` (o original), e a cópia nunca muda.
- **Correção**: tratar o nome do arquivo como fonte do id (ou rejeitar divergência como arquivo
  com problema) e documentar como duplicar fichas.

## BE-6 · Baixa — `PUT` sem `version` responde 409
- `{"name":"x","draft":{}}` sem `version` → 409 "alterada em outra aba". Deveria ser 400
  (campo obrigatório); hoje `version` assume 0.

## BE-7 · Baixa — sem `abilityMethod`, valores base até 20 são aceitos
- Com método, o teto é 15 ou 18; sem método qualquer valor de 1 a 20 passa. O front sempre envia
  o método, mas a API aceita fichas que nenhum método do LdJ (p. 13) produz.

## BE-8 · Baixa — mensagens com ids crus
- Erro de escolha racial lista `skill:acrobacia, skill:arcanismo, …` e `ability:forca`.
  Para o público iniciante, usar os nomes ("Acrobacia", "Força").

## Pontos verificados sem problema
- 404 com ProblemDetails em pt-BR para raça/classe/ficha inexistente; GUID inválido na rota → 404.
- Validação de nome (vazio, só espaços, 81 caracteres) e `trim` do nome.
- Conflito otimista: 5 `PUT` paralelos com a mesma versão → 1×200 e 4×409.
- 30 `POST` concorrentes → 30 arquivos, sem `.tmp` sobrando.
- Ficha com raça removida do catálogo: `GET` devolve `sheet: null` + `errors` (ADR-0006 item 2).
- Teto de atributo 20 com bônus racial aparece no breakdown ("limitado ao teto de 20").
- Regras de sobreposição de perícia (raça × classe × antecedente) geram 400 claro.
