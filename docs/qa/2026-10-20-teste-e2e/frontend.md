# Front-end — bugs encontrados (20/10/2026)

Voltar ao [resumo](README.md). Reproduzidos com Playwright no Chromium (front isolado + API isolada).

## FE-1 · Alta — erros de validação do salvamento nunca aparecem
- **Reproduzir**: Atributos → "Compra de pontos" → 15 em todos (90 pontos). Status diz
  *"Não salvo: corrija os campos destacados."*, mas **nenhum campo é destacado e nenhuma
  mensagem aparece**; o botão "Próximo" continua ativo.
- **Causa**: no 400 do `PUT`, a API devolve as chaves com prefixo (`draft.baseAbilities`,
  `draft.expertise` — `SheetInput.Validate`). Os passos filtram `baseAbilities`,
  `skillChoices`, `extraProficiencies`, `expertise` sem o prefixo
  (`abilities-step.ts` `formErrors`, `skills-saves-step.ts` `fieldErrors`).
- **Por que os testes não pegaram**: `character-draft.store.spec.ts` simula o 400 com a chave
  `level` (sem prefixo) — o contrato real não é exercitado; o E2E não cobre estados inválidos.
- **Correção**: remover o prefixo `draft.` no store (ou padronizar na API) + teste unitário
  com a chave real + um E2E de compra de pontos estourada.

## FE-2 · Média — passos Raça e Classe não mostram nenhum erro
- **Reproduzir**: Ladino nv. 6 com 4 Especializações → Classe → nível 3. Status: "Não salvo",
  sem explicação no passo; as mudanças seguintes também não salvam até o usuário descobrir,
  sozinho, que deve desmarcar Especializações no passo IV.
- Mesmo efeito esperado ao trocar para uma raça que já dá uma perícia escolhida na classe
  (ex.: Percepção de Ladino → Elfo): a API recusa com "já é proficiente" (verificado via API).
- **Correção**: ao baixar o nível, cortar Especializações excedentes (como `pickClass` já
  limpa perícias) ou mostrar os erros de qualquer campo no topo do wizard com link ao passo.

## FE-3 · Média — valor decimal mostra "o servidor não respondeu"
- Atributos (compra de pontos/manual): digitar `12.5` → API 500 (BE-3) → status
  *"Não foi possível salvar: o servidor não respondeu"*, o que é falso e assusta o iniciante.
- **Correção**: `step="1"` + arredondar/recusar no `setBase`; tratar 4xx/5xx de forma distinta.

## FE-4 · Média — "Minhas fichas": criar/apagar sem tratamento de erro
- Com a API fora, "Nova ficha" não faz nada visível; console: `ERROR HttpErrorResponse`
  (rejeição não tratada em `create()`). `remove()` tem o mesmo padrão.
- **Correção**: `try/catch` com mensagem no `role="alert"` já existente.

## FE-5 · Média — toda falha ao abrir vira "Ficha não encontrada"
- API fora do ar ou arquivo corrompido (500) → "Ficha não encontrada". O `catch` do
  `WizardShellComponent.load` não olha o status. Diferenciar 404, 0/5xx e arquivo com problema.

## FE-6 · Média (a11y) — diálogos sem gestão de foco
- "Apagar ficha?" e "alterada em outra aba" usam `role="alertdialog" aria-modal="true"`, mas o
  foco não vai para o diálogo, **Esc não fecha** e o foco não fica preso. O axe não detecta isso.
- **Correção**: `<dialog>` nativo com `showModal()` (foco, Esc e inércia de graça) ou CDK A11y.

## Baixa severidade
- **FE-7** — Mensagens de erro do status usam `color: var(--success)` (verde) — ver capturas.
- **FE-8** — Renomear para vazio: ignorado em silêncio; o campo fica vazio e o servidor mantém
  o nome antigo. Restaurar o valor ou avisar.
- **FE-9** — "Próximo" fica ativo com o salvamento inválido.
- **FE-10** — Lista mostra "nível 1" quando o nível ainda não foi escolhido (`s.level ?? 1`).
- **FE-11** — `classFeatures` em `effect` sem cancelamento: trocar nível rápido pode exibir
  resposta antiga; promessas do `RulesetService` sem `catch` nos passos.
- **FE-12** — `API_BASE_URL` fixo em `http://localhost:5080` (e repetido na mensagem de erro):
  sem config por ambiente; o E2E precisa reescrever rotas para usar outra porta.
- **FE-13** — `flush()` não é usado e não há aviso `beforeunload`: fechar a aba < 600 ms após
  uma mudança perde a mudança (navegar dentro do app funciona — verificado).
- **FE-14** — Cosmético em "Meus bônus › Escolhas": "Aumento de Atributo (Meio-Elfo) (Meio-Elfo)".

## Pontos verificados sem problema
- Jornada completa (criar → Raça com escolhas → Classe → Atributos → Perícias → Ficha) com
  salvamento automático e valores corretos; Furtividade +7 e PV 24 conferidos à mão.
- Trocar de raça limpa sub-raça e escolhas, com aviso; Especialização em perícia que continua
  proficiente pela nova raça permanece válida.
- Ficha inexistente / id inválido na URL → "Ficha não encontrada" com link de volta.
- Lista com arquivo corrompido renderiza; ids duplicados (BE-5) não geram erro no console.
- Tema escuro e 375 px sem rolagem horizontal; axe sem violações nos passos testados.
