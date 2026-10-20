# ADR-0006: Fichas salvas como arquivos JSON no servidor (sem banco)

- **Status**: aceito — substitui a parte de persistência do ADR-0002
- **Data**: 2026-10-20
- **Decisores**: Gabriel Portillo

## Contexto
O ADR-0002 deixou o rascunho só no navegador (`localStorage`). O autor quer **salvar as
fichas**, ver no front as fichas já existentes e que toda edição altere a própria ficha
salva — ainda sem banco de dados e sem login. O deploy é só local (AD-012).

## Decisão
1. Cada ficha é **um arquivo JSON** numa pasta do servidor: `<SheetsPath>/<id>.json`.
   `SheetsPath` vem do `appsettings` (padrão `data/fichas/` na raiz do repo, fora do git).
2. O arquivo guarda **só as escolhas** (`CharacterDraft`) + metadados (`id`, `name`,
   `version`, `createdAt`, `updatedAt`, `schemaVersion`). A ficha derivada **não** é salva:
   é recalculada pelo `Domain` a cada leitura, então correções no catálogo valem para todas.
3. Porta `ICharacterSheetStore` na `Application`; `FileCharacterSheetStore` na
   `Infrastructure`. O `Domain` continua puro (sem I/O) e dono de todo cálculo.
4. `id` é GUID gerado pelo servidor (impede *path traversal*). Gravação atômica
   (arquivo temporário + rename). Concorrência otimista: `PUT` envia `version`; divergiu → `409`.
5. Ficha incompleta pode ser salva (o wizard é progressivo); escolhas pendentes viram
   `warnings`. Valores inválidos (ex.: nível 0) → `400`.
6. O front lista, cria, abre, edita (salvamento automático com debounce) e apaga fichas pela
   API. O `localStorage` deixa de ser a fonte da verdade.

## Alternativas consideradas
- SQLite/PostgreSQL — exige migrações e driver; sem ganho para uso local de 1 pessoa.
- Só `localStorage`/IndexedDB — fichas presas a um navegador, não viram arquivos.
- Salvar também a ficha calculada — fica desatualizada quando uma regra é corrigida.

## Consequências
- **Positivas**: fichas legíveis e versionáveis à mão; zero infraestrutura nova; nenhuma
  dependência nova (`System.Text.Json` + `System.IO`).
- **Negativas / trade-offs**: não escala para vários usuários nem servidores; sem busca;
  a pasta precisa de backup manual.
- **Follow-ups**: multiusuário, login ou compartilhamento por link → novo ADR (banco + auth).
  Mudança no formato do arquivo → incrementar `schemaVersion` e migrar na leitura.
