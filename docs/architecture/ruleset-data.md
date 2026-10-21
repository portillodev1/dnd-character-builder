# Catálogo de regras (`content/`)

Regras do jogo como **dados versionados** (ADR-0003). Regras para a IA:
`rules/ruleset-content.md`. Licença do conteúdo: ADR-0004.

## Layout
```txt
content/
  schemas/
    race.schema.json · class.schema.json · skills.schema.json · common.schema.json
    (draft 2020-12; validados com JsonSchema.Net — ADR-0005)
  races/      anao.json · elfo.json · halfling.json · humano.json · draconato.json
              gnomo.json · meio-elfo.json · meio-orc.json · tiefling.json
  classes/    barbaro.json · bardo.json · bruxo.json · clerigo.json · druida.json
              feiticeiro.json · guerreiro.json · ladino.json · mago.json · monge.json
              paladino.json · patrulheiro.json
  skills.json # 18 perícias → atributo-chave (+ sinônimos do livro)
```

## Entidades
| Entidade | Campos-chave |
|---|---|
| `Race` | `id`, `name`, `summary`, `size`, `speed {meters, note?}`, `bonuses[]`, `choices[]`, `traits[]`, `languages[]`, `subraces[]` (embutidas), `source` |
| `Subrace` | `id`, `name`, `summary`, `srd` (false = fora do SRD, ADR-0004), `bonuses[]`, `choices[]`, `traits[]`, `source`; somada à raça-mãe |
| `Class` | `id`, `name`, `summary`, `hitDie`, `primaryAbilities[]`, `savingThrows[2]`, `skillChoices {count, from[]}`, `proficiencies {armor, weapons, tools}`, `subclassLevel`, `table[] {id, name, values[20]}`, `features[]`, `source` |
| `ClassFeature` | `id`, `name`, `level`, `kind`, `description`, `scaling?` (coluna de `table`), `grants[]` (`expertise`, `half-proficiency`, `skill-proficiency`, `save-proficiency`, `ability-score-improvement`, `subclass-choice`), `source` |
| `Bonus` | `target` (`ability:x`, `skill:x`, `save:x`, `speed`, `hp-per-level`, `ac`), `value`, `bonusType`, `label?`, `source` |
| `Trait` | `id`, `name`, `kind` (`sense`, `resistance`, `advantage`, `proficiency`, `language`, `spell`, `other`), `description`, `source` |

## Escolhas do jogador
Bônus que exigem escolha (ex.: Meio-Elfo +1 em dois atributos; Versatilidade em Perícia)
são modelados como `choice { count, from[], bonusType, value? }`. Com `bonusType`, o Domain
aplica cada opção escolhida: `ability:x` soma `value` (padrão 1) como aumento de atributo e
`skill:x` dá proficiência com origem na raça. Sem `bonusType` (idioma, ferramenta, ancestral),
a escolha só é registrada. A API devolve `warnings` enquanto a escolha não for feita.

## Escopo de subclasses
MVP: só características **da classe base** e o marco "escolha seu arquétipo no nível N".
Arquétipos: todos os 41 do LdJ, em fatias por classe (SRD `srd: true`, demais `srd: false`) —
[ADR-0011](../adr/ADR-0011-archetype-scope.md); licença no ADR-0004.

## Integridade (testada em `Ruleset.Tests`)
- Todo JSON valida no schema; `id` único por pasta; referências (`subraces`, `skill:x`) existem.
- Toda classe tem exatamente 2 salvaguardas e ≥ 1 feature no nível 1.
- Todo `source.page` está dentro do intervalo do capítulo correspondente.
