# T2 (Fase 9) — verificação de F1–F5 no app

Data: 2026-10-21 · Como: `api` (5080) + `web` (4200) no navegador do app, ficha de teste
"Meio-Elfo · Ladino nv. 3" (criada e apagada por mim; fichas Gomez e Lia intactas).
Console do navegador e log da API sem erros.

## Resultado: F1–F5 atendidos, sem bloqueio. Dois achados de UX (baixa/média).

| Critério | Verificado | OK |
|---|---|---|
| F1 AC1–2 | 9 raças com resumo e aumentos; Anão exige sub-raça e escolha de ferramenta antes de "Próximo" | ✅ |
| F1 AC3–5 | Traços agrupados por origem (Anão / Anão da Colina), com tipo, texto e página | ✅ |
| F1 AC6–7 | Meio-Elfo bloqueia avanço até 2 atributos, 2 perícias e 1 idioma; trocar de raça limpa e avisa | ✅ |
| F2 AC1–8 | 12 classes; Ladino nv. 1 sem Ação Ardilosa (aparece em "Próximo nível"); nv. 3 com marco do arquétipo; nv. 20 sem "Próximo nível"; Ataque Furtivo 1d6→2d6→10d6 | ✅ |
| F3 AC1–9 | Base 15 + Meio-Elfo → Des 16; Acrobacia +7 com Especialização (3+2×2); perícias fora da lista desabilitadas com motivo; racial travada; contadores 4/4, 2/2, 2/2; passiva 12; painel "Como preencher" | ✅ |
| F4 AC1–3 | 40 números clicáveis (botões); Des 16 = 15 base + 1 racial; PV 24 = 6 + 8 + 10, com tipo e página | ✅ |
| F4 AC4–7 | "Meus bônus" por origem; benefícios não numéricos; legenda; Gomez (Anão da Colina): Con 13+2=15, PV 8+2+1 (Tenacidade Anã) | ✅ |
| F5 AC1–4, 7–8 | Lista ordenada com raça/classe/nível/data; nova ficha abre em Raça; reabrir restaura tudo; duplicar cria "(cópia)"; apagar pede confirmação com o nome e remove o arquivo; renomear mantém `id` | ✅ |
| F5 AC5–6 | Coberto pelos E2E (22 Playwright) e Api.Tests; não repeti 409 com duas abas | — |

## Achados
1. **Painéis "Ver a conta" empilham e cobrem outros números (média).** Só fecham clicando
   de novo no mesmo botão: Esc e clique fora não fecham. O painel de Pontos de Vida cobre a
   coluna de Perícias e bloqueia o clique em Furtividade até fechá-lo.
   Sugestão: fechar com Esc e ao abrir outro/clicar fora (também ajuda WCAG 1.4.13).
2. **Renomear só salva ao sair do campo (baixa).** Digitar o novo nome não grava; só grava
   no blur (confirmado: versão 14→15 e `name` atualizado após Tab). Quem fechar a aba logo
   após digitar perde o nome. Sugestão: salvar com debounce como as demais escolhas.

## Fora do que testei
Navegação por teclado e leitor de tela (cobertos pelo axe/E2E, não manualmente); tema claro;
telas estreitas; F5 AC6 (409).
