# STATE.md — memória entre sessões

Fonte única de estado e handoff entre áreas (Harness → Spec → Loop). Leia o
`## Handoff
- **Fase 9 (em andamento)**: tasks em `.specs/project/tasks-fase-9.md`. A: T1–T5 feitas (QA em `docs/qa/2026-10-21-*`; T3 = `6bfaf75`; T4 aprovada e tasks 1–5 apagados;
  push feito). B: F11 `class-speed-bonus` implementada (T6–T10; Bárbaro p. 49, Monge p. 104),
  aguardando revisão humana. C: escopo decidido (T11, ADR-0011: todos os 41 arquétipos do LdJ, em fatias por classe);
  F12 `class-archetypes`: contrato `draft.archetype` aprovado e back-end pronto (T13; Campeão e
  Linhagem Dracônica como dados-piloto). Front pronto (T14): escolha no passo Classe e "Meus bônus". Dados: T15 feita (12 arquétipos do SRD, 1 por classe, todas as classes escolhíveis);
  T16 feita: os 41 arquétipos do LdJ (12 SRD + 29 fora) no catálogo, nomes/níveis/páginas conferidos
  por script nas colunas do PDF. Resta o humano revisar o conteúdo não-SRD antes de publicar o repositório.
- **Última unidade**: **Fase 8 aprovada** (2026-10-21) e limpa — spec F10 permanece;
  `tasks-fase-8.md` apagado. Commits: spec `ed4b2df`, implementação `30204f1`.
- **Fase 7 (anterior)**: aprovada (2026-10-21); F6 `6fa259a`, F7 `027d66e`, F8 `e49f055`, F9 `2f088fe`.
  Dev: reinicie o `ng serve` para usar o `proxy.conf.json` (ADR-0010).
- **QA do MVP (anterior)**: `docs/qa/2026-10-20-teste-e2e/`; FE-1 e BE-1 corrigidos (`34749c3`,
  já no `origin/main`).
- **Fase 6 (anterior)**: E2E + a11y (ADR-0009).
  - `frontend/e2e/`: 22 testes Playwright cobrindo F1–F5 + axe (WCAG 2.2 AA) em todos os passos,
    claro e escuro, e navegação por teclado. `scripts/e2e.sh` sobe API (5180, fichas em
    `frontend/e2e/.data/`) e front (4300) isolados; entra no `verify.sh` e no CI.
  - Polimento: "Minhas fichas" mostra nomes de raça/classe; passo Raça usa o deslocamento da
    sub-raça ("10,5 m"); mensagens de erro de servidor em linguagem de iniciante.
- **Lacunas F5 (anterior)**: deslocamento com breakdown, bônus de escolha com origem própria e
  limite de Especialização (`20ab4cb`).
- **Design do front** (polimento da Fase 6, adiantado): estética de manuscrito — velino com grão,
  rubricas em vermelho, azul-lápis, filetes duplos, capitular, passos em numerais romanos; tema
  escuro "à luz de vela". Fontes EB Garamond + IM Fell English via @fontsource (ADR-0008). Ícones
  de tipo de bônus agora são sinais tipográficos (sem emoji). Já no `origin/main`.
- **Fase 5 (anterior)**: wizard Angular + "Minhas fichas" (tasks apagados na limpeza); tipos do
  front gerados do OpenAPI (ADR-0007); salvamento automático; `bonus-breakdown` em todo número.
  Testes: 20 front, 248 back.
- **Rodar**: `.claude/launch.json` (local, não versionado) tem `api` (5080) e `web` (4200).
- **Lacunas conhecidas**: aumentos de deslocamento de classe (Monge, Bárbaro) — resolvido na F11.
- **Revisão humana**: Fases 1–6 (MVP completo) aprovadas em 2026-10-21; pendente só a revisão do conteúdo
  não-SRD (29 arquétipos) antes de publicar o repositório.
- **Toolchain local (macOS)**: Node 24 via `brew install node@24` (keg-only) → exporte
  `PATH="/opt/homebrew/opt/node@24/bin:$PATH"` antes dos scripts. PDF do LdJ fica fora do repo
  (ADR-0004); para conferir páginas use `pdftotext` (poppler) num diretório temporário.
- **Bloqueios**: nenhum.
- **Próxima unidade**: revisão humana das Fases 1–6; depois o backlog do `ROADMAP.md`
  (arquétipos tirariam o último selo "anote à mão" — exige spec e decisão de ADR-0004).
- **Limpeza**: `tasks-fase-1…5` apagados na aprovação de 2026-10-21; resta `tasks-fase-9.md` (apagar quando a Fase 9 for aprovada).
- **Gates**: `verify.sh --strict` verde.
