# Spec F8: O wizard sempre diz o que está acontecendo

Pós-MVP (Fase 7). Origem: relatório `docs/qa/2026-10-20-teste-e2e/` — FE-2…FE-13.
Depende de F6 (todo 4xx é `ValidationProblem`) e F7 (`422` para arquivo com problema).
FE-1 (prefixo `draft.`) já corrigido em `34749c3`.

## Objective
O iniciante nunca fica preso: todo salvamento recusado diz **o quê** e **onde** corrigir, falhas
de rede são distinguidas de erros de escolha, e nenhuma mudança se perde em silêncio.

## User stories
- Como iniciante, quero ver por que minha ficha não foi salva e ir direto ao passo que resolve.
- Como jogador, quero saber se o problema é meu (escolha) ou do servidor (rede).
- Como usuário de teclado/leitor de tela, quero diálogos que recebem o foco e fecham com Esc.

## Acceptance criteria
1. **Dado** um salvamento recusado (`400`), **então** um resumo no topo do wizard
   (`role="alert"`) lista cada erro com link para o passo do campo (Raça: `raceId`, `subraceId`,
   `choices.*`; Classe: `classId`, `level`; Atributos: `abilityMethod`, `baseAbilities*`;
   Perícias: `skillChoices`, `extraProficiencies`, `expertise`; Nome: `name`).
2. **Quando** baixo o nível e a classe passa a conceder menos Especializações (`expertiseCount`
   da API), **então** as excedentes são desmarcadas (mantidas as primeiras) e um aviso diz quais.
3. Campos numéricos de atributo aceitam só inteiros: `12.5` ou texto mostram "Use um número
   inteiro" junto ao campo e **não** são enviados.
4. O status do salvamento usa cor de perigo nos estados `invalid`, `conflict` e `error`;
   no estado `error` há o botão "Tentar de novo" que repete o último salvamento.
5. `400` → "Não salvo: …" (escolha); rede/`5xx` → "o servidor não respondeu" (só nesses casos).
6. **Quando** apago todo o nome da ficha, **então** o campo volta ao nome anterior e aparece
   "O nome da ficha não pode ficar vazio."
7. "Próximo" fica desabilitado, com o motivo, enquanto o status for `invalid` ou `conflict`.
8. Abrir ficha: `404` → "Ficha não encontrada"; `422` → "O arquivo desta ficha está com
   problema: <detail>"; rede/`5xx` → "O servidor não respondeu" com "Tentar de novo".
9. "Minhas fichas": falha ao criar/apagar/duplicar mostra mensagem em `role="alert"` e o botão
   volta a funcionar (sem erro não tratado no console).
10. Na lista, ficha sem nível mostra "nível a escolher" (não "nível 1").
11. Trocar nível/classe rápido mostra as características do **último** valor escolhido
    (respostas antigas de `classFeatures` são descartadas).
12. **Dado** status `pending`/`saving`, **quando** fecho/recarrego a aba, **então** o navegador
    pede confirmação (`beforeunload`).
13. Diálogos "Apagar ficha?" e "alterada em outra aba": `<dialog>` modal com foco inicial no
    diálogo, Tab preso dentro, foco volta ao botão de origem; Esc fecha o de apagar (= Cancelar).

## Requisito não funcional
- A origem da API vem de configuração (`proxy.conf.json` no `ng serve`, mesma origem em
  produção) — sem `http://localhost:5080` no código; o E2E deixa de reescrever rotas. ADR-0010.

## Testes exigidos
- Vitest: AC1 (mapa campo → passo), AC2, AC3, AC4, AC6, AC7, AC8 (404/422/0), AC9, AC10, AC11.
- E2E: AC1 (baixar nível do Ladino 6 → 3 com 4 Especializações → aviso de AC2, salvo);
  AC8 com API fora; AC13 por teclado (foco, Tab, Esc); axe nos novos estados, claro e escuro.

## Out of scope
Desfazer/refazer; modo offline com fila de salvamentos; edição colaborativa.
