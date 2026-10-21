# Spec F6: Entrada inválida nunca derruba a API

Pós-MVP (Fase 7). Origem: relatório `docs/qa/2026-10-20-teste-e2e/` — BE-3, BE-4, BE-6, BE-7, BE-8.
Contrato: `docs/architecture/api-contract.md` § Validação. **Muda o contrato público** (AC3, AC5):
exige aprovação humana antes de implementar (AGENTS.md › PERGUNTE QUANDO).

## Objective
Toda requisição malformada ou com valor fora do esperado recebe **400 com ProblemDetails em
pt-BR**, nunca 500. Mensagens usam os nomes da ficha ("Acrobacia", "Força"), não ids internos.

## User stories
- Como jogador, quero entender o que está errado na minha escolha, sem "erro interno".
- Como dev do front, quero que todo 4xx tenha o formato `ValidationProblem` para exibi-lo.

## Acceptance criteria
1. **Dado** um corpo que não é JSON válido, vazio ou com tipo errado (`"level":"3"`,
   `"forca":12.5`, `"forca":null`, número acima de `int`), **quando** chamo qualquer `POST`/`PUT`,
   **então** recebo `400` `application/problem+json` com `title` em pt-BR (não `500`).
2. **Dado** `GET /classes/{id}/features` sem `level`, **então** `400` com
   `errors.level = ["O nível deve estar entre 1 e 20."]` (mesma mensagem de `level=0`); com
   `level=abc`, `400` em `errors.level` ("Valor em formato inválido para este campo.").
3. **Dado** `choices: { "<id-válido>": null }`, **então** a escolha conta como não feita
   (vira `warning`, como lista vazia); item nulo dentro da lista → `400` em `choices.<id>`.
   Item nulo em `skillChoices`/`expertise`/`extraProficiencies` → `400` "Perícia vazia na lista".
4. **Dado** `baseAbilities` **sem** `abilityMethod`, **então** cada valor deve estar entre 3 e 18
   (maior faixa entre os métodos — LdJ p. 13); fora disso → `400` em `baseAbilities.<id>`.
5. **Dado** `PUT /character-sheets/{id}` sem `version`, **então** `400` em `errors.version`
   ("Envie a versão da ficha que você abriu."), não `409`.
6. Mensagens de escolha racial listam opções pelo nome: "escolha 2 entre Acrobacia, Arcanismo…"
   e "Força, Destreza…" (não `skill:acrobacia`/`ability:forca`).
7. O OpenAPI (`/openapi/v1.json`) continua documentando `400` (`HttpValidationProblemDetails`)
   em todos os endpoints com corpo ou query.

## Decisões
- Erro de binding (`BadHttpRequestException`) → `BadRequestExceptionHandler` (`IExceptionHandler`)
  escreve `ValidationProblem` com o campo do caminho do JSON ou do parâmetro; `ThrowOnBadRequest`
  ligado em todo ambiente para produção responder igual a Development.
- `level` da query passa a `int?` validado pelo caso de uso (mesmo `Result.Invalid` de hoje).
- Nomes de perícia/atributo vêm de `Skill.DisplayName()`/`Ability.DisplayName()` (Domain).

## Testes exigidos
- `Api.Tests` (Theory): cada linha da tabela de BE-3 do relatório → `400` + `problem+json`.
- `Api.Tests`: `choices` nulo → `200` com warning; item nulo → `400`; `PUT` sem `version` → `400`.
- `Domain.Tests`: faixa 3–18 sem método (limites 2, 3, 18, 19); mensagem com nomes.
- `OpenApiSnapshotTests` atualizado + `scripts/gen-api.sh` se o contrato mudar.

## Out of scope
Rate limiting, limite de tamanho de corpo, autenticação.
