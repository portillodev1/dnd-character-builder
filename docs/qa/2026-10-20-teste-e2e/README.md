# Teste de ponta a ponta — 20/10/2026

Relatório de QA do MVP (Fases 1–6, commit `b60381b`). Dividido em arquivos ≤ 100 linhas:

| Arquivo | Conteúdo |
|---|---|
| este `README.md` | Como foi testado, resultado dos gates e resumo |
| [backend.md](backend.md) | Bugs da API / Domain / Infrastructure, com reprodução |
| [frontend.md](frontend.md) | Bugs do Angular, com reprodução |
| [melhorias.md](melhorias.md) | Melhorias sugeridas (UX, testes, arquitetura) e prioridade |

## Como foi testado
1. **Gate oficial**: `scripts/verify.sh --strict` (docs, build, lint, testes, E2E + axe).
2. **API exploratória**: ~45 requisições `curl` contra uma API isolada (porta 5380, fichas numa
   pasta temporária — `data/fichas` real não foi tocada), cobrindo entradas inválidas,
   arquivos de ficha corrompidos/editados à mão, conflito de versão e concorrência.
3. **Front exploratório**: roteiro Playwright próprio (fora do repo) contra front isolado (4400)
   com as chamadas a `:5080` redirecionadas para a API isolada — mesma técnica do `e2e/support.ts`.
   Jornada completa de iniciante + cenários de borda, capturas de tela, axe, tema escuro e 375 px.

## Resultado dos gates (`verify.sh --strict` → exit 0)
| Etapa | Resultado |
|---|---|
| check-docs | OK |
| Build back (.NET 10, Release) | OK — 0 avisos, 0 erros |
| Build front (Angular) | OK — bundle inicial 282 kB (77 kB transferidos) |
| Lint (dotnet format, ESLint, Prettier) | OK |
| Testes back (Domain, Ruleset, Api) | **248/248** aprovados |
| Testes front (Vitest) | **22/22** aprovados |
| E2E Playwright + axe | **22/22** aprovados (26,8 s) |

## Resultado do teste exploratório
**35 verificações de UI: 26 ok, 9 falharam.** API: 8 problemas encontrados.

O que funcionou bem:
- **Cálculos corretos** na jornada Meio-Elfo Ladina nv. 3 (conjunto padrão 8/15/13/12/10/14,
  +1 Des/+1 Con, +2 Car): Des 16 (+3), Con 14 (+2), PV **24** (8 + 5 + 5 + 2×3), BP +2,
  Furtividade com Especialização **+7**, Percepção passiva **14**, salvaguardas Des +5 / Int +3.
- Breakdown "Ver a conta" em todos os números; páginas do LdJ citadas.
- **axe: 0 violações** (Raça com escolhas, Perícias, Ficha claro e escuro).
- **Mobile 375 px**: sem rolagem horizontal em Raça, Atributos, Perícias e Ficha.
- Conflito de versão (409) e 30 criações concorrentes: corretos, sem `.tmp` órfão.
- Arquivo JSON inválido aparece na lista como "arquivo com problema" (como manda a spec F5).
- Sair do wizard logo após uma mudança: a mudança é gravada.
- Nome com `<script>` é exibido como texto (Angular escapa) — sem XSS.

## Resumo dos problemas (detalhes nos arquivos)
| # | Sev. | Área | Problema |
|---|---|---|---|
| FE-1 | **Alta** | Front | ✅ Corrigido — erros do salvamento não apareciam na tela (prefixo `draft.`) |
| BE-1 | **Alta** | API | ✅ Corrigido — um arquivo sem `draft` derrubava "Minhas fichas" inteira (500) |
| FE-2 | Média | Front | Passos Raça/Classe não mostram erros; baixar nível trava o salvamento em silêncio |
| BE-2 | Média | API | Abrir/salvar ficha com arquivo corrompido → 500 |
| BE-3 | Média | API | JSON malformado, tipo errado ou `level` ausente → 500 (deveria ser 400) |
| BE-4 | Média | API | `choices: {"<id>": null}` → 500 |
| BE-5 | Média | API | Arquivo copiado (id interno ≠ nome) → ids duplicados e gravação no arquivo errado |
| FE-3 | Média | Front | Valor decimal em atributo → "o servidor não respondeu" |
| FE-4 | Média | Front | Criar/apagar ficha com API fora: erro não tratado, sem aviso |
| FE-5 | Média | Front | Qualquer falha ao abrir ficha vira "Ficha não encontrada" |
| FE-6 | Média | a11y | Diálogos sem foco inicial, sem Esc e sem prender o foco |
| BE-6…8, FE-7…14 | Baixa | — | Ver arquivos de detalhe |

## Ambiente
macOS (Darwin 25.5), Node 24.22, .NET SDK 10.0.402, Chromium do Playwright.
Nenhum arquivo de código foi alterado; servidores de QA encerrados ao final.
