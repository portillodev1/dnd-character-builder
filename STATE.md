# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff
- **Última unidade**: lacunas da Fase 5 fechadas (specs F4 AC1/AC4, F3 AC5). Commit local; **push pendente**.
  - Deslocamento: `SpeedRules` + `DerivedMeasure` (metros, decimal) → `sheet.speed` com breakdown
    (Elfo da Floresta = 9 + 1,5 Pés Ligeiros, p. 23–24); sub-raça ganhou `speed` opcional no catálogo.
  - Escolhas: `origin: "choice"` em `bonuses[]` (+1 Meio-Elfo; perícias de raça/classe/antecedente e
    Especialização, valor = Bônus de Proficiência) e em `traits[]` (idioma/ferramenta/resistência);
    "Meus bônus › Escolhas" não lê mais o rascunho.
  - Especialização: `expertiseCount` em `/classes/{id}/features?level=N`; o front trava acima do limite.
- **Design do front** (polimento da Fase 6, adiantado): tokens de cor/tipografia claro+escuro em
  `frontend/src/styles.scss`; cabeçalho, stepper numerado, cards de raça/classe, painéis da ficha,
  tabela de perícias com rolagem no celular. Só estilos — nenhuma lógica mudou. Commit local; **push pendente**.
- **Fase 5 (anterior)**: wizard Angular + "Minhas fichas" (`.specs/project/tasks-fase-5.md`); tipos do
  front gerados do OpenAPI (ADR-0007); salvamento automático; `bonus-breakdown` em todo número.
  Testes: 20 front, 248 back.
- **Rodar**: `.claude/launch.json` (local, não versionado) tem `api` (5080) e `web` (4200).
- **Lacunas conhecidas**: passo Raça ainda mostra o deslocamento da raça-mãe (a ficha já usa o da
  sub-raça); aumentos de deslocamento de classe (Monge, Bárbaro) fora do MVP.
- **Revisão humana pendente**: Fases 1–5.
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: Fase 6 (Playwright E2E dos 5 critérios, axe, polimento de textos; visual já polido).
- **Limpeza**: apagar `tasks-fase-1.md` … `tasks-fase-5.md` quando aprovadas.
- **Gates**: `verify.sh --strict` verde.
