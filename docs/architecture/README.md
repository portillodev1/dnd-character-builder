# Arquitetura — visão geral

## Problema
Iniciantes não sabem **de onde vem cada número** da ficha. O produto não só calcula:
ele **explica** cada valor (origem + tipo de bônus), seguindo o Livro do Jogador (LdJ).

## Visão de contêineres (C4 nível 2)
```txt
┌──────────────┐  HTTPS/JSON   ┌──────────────────────────────┐
│  Navegador   │ ────────────► │  API .NET 10 (stateless)     │
│  Angular 22  │ ◄──────────── │  Minimal APIs + OpenAPI      │
│  SPA wizard  │               │  ┌────────────────────────┐  │
│  rascunho em │               │  │ Domain: motor de regras│  │
│  localStorage│               │  └────────────────────────┘  │
└──────────────┘               │  catálogo JSON (embedded)    │
                               └──────────────────────────────┘
```

## Decisões estruturantes
| Decisão | Por quê | ADR |
|---|---|---|
| Monorepo `frontend/` + `backend/` + `content/` | 1 PR muda contrato e consumidor juntos | ADR-0001 |
| API stateless, sem banco no MVP | DoD não exige salvar ficha; rascunho fica no navegador | ADR-0002 |
| Cálculo só no back-end (`Domain`) | Uma fonte da verdade, testável por tabela | ADR-0002 |
| Catálogo de regras como JSON versionado + schema | Conteúdo revisável sem recompilar lógica | ADR-0003 |
| Conteúdo baseado no SRD 5.1; LdJ só como referência de página | Licença | ADR-0004 |

## Fluxo principal (uma ida e volta por passo do wizard)
1. Front pede catálogo: `GET /api/v1/races`, `GET /api/v1/classes`.
2. Usuário escolhe raça/sub-raça, classe, nível, atributos, perícias.
3. Front envia o rascunho: `POST /api/v1/character-sheets/compute`.
4. API devolve a ficha derivada **com breakdown** de cada valor.
5. Front renderiza a ficha no layout da ficha oficial; cada número abre "como calculei".

## Qualidades priorizadas
- **Correção das regras** > performance (testes de tabela por fórmula).
- **Explicabilidade**: todo `DerivedValue` tem `parts[]` somando o total.
- **Simplicidade**: sem auth, sem DB, sem filas. Adicionar só via ADR.

## Detalhe
[backend.md](backend.md) · [frontend.md](frontend.md) · [api-contract.md](api-contract.md) ·
[ruleset-data.md](ruleset-data.md)
