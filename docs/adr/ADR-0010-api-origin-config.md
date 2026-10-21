# ADR-0010: Origem da API por configuração (proxy no dev, mesma origem fora dele)

- **Status**: aceito
- **Data**: 2026-10-20
- **Decisores**: dono do projeto (aprovação da Fase 7) + agente

## Contexto
O front fixa `http://localhost:5080` em `API_BASE_URL` e numa mensagem de erro. Rodar a API em
outra porta exige editar código; o E2E (ADR-0009) contorna isso reescrevendo cada requisição
`:5080` para `:5180` com `context.route`; e o relatório de QA (FE-12) precisou da mesma gambiarra.
Também obriga CORS entre `:4200` e `:5080` em desenvolvimento. Spec F8, requisito não funcional.

## Decisão
O front chama a API por **caminho relativo** (`/api/v1/...`, `/health`): `API_BASE_URL` passa a
`''` por padrão.
- **Dev**: `ng serve` usa `frontend/proxy.conf.json` (`/api` e `/health` → `http://localhost:5080`),
  recurso nativo do Angular CLI — sem dependência nova.
- **E2E**: `ng serve --proxy-config proxy.e2e.conf.json` apontando para a API isolada (`:5180`);
  `e2e/support.ts` deixa de reescrever rotas.
- **Produção** (fora do MVP): front e API na mesma origem (reverse proxy ou a API servindo o
  `dist/`); se um dia forem origens diferentes, `API_BASE_URL` é fornecido por `provide` no
  `app.config.ts` a partir de configuração de build.
- Mensagens ao usuário não citam URL; dizem "a API (servidor) não respondeu".

## Alternativas consideradas
- `environment.ts` por configuração de build — exige rebuild por porta e não resolve o E2E com
  `ng serve`; o proxy resolve os dois sem arquivos por ambiente.
- `config.json` carregado em runtime (`APP_INITIALIZER`) — flexível, mas adiciona uma requisição
  e um estado de "configuração carregando" desnecessários para app de mesma origem.
- Manter a reescrita no Playwright — esconde o acoplamento e não ajuda o dev local.

## Consequências
- **Positivas**: porta da API muda só no proxy; E2E e QA testam o front como ele roda; sem CORS
  no dev (mesma origem), o que simplifica erros de rede.
- **Negativas / trade-offs**: a API rodando sozinha não é acessível pelo front sem o proxy
  (`ng serve` é obrigatório no dev); `launch.json`/README precisam citar o proxy.
- **Follow-ups**: T26 do `tasks-fase-7.md` (implementação); revisar se a política de CORS da API
  ainda é necessária depois disso.
