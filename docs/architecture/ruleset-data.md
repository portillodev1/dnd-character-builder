# Catálogo de regras (`content/`)

Regras do jogo como **dados versionados** (ADR-0003). Regras para a IA:
`rules/ruleset-content.md`. Licença do conteúdo: ADR-0004.

## Layout
```txt
content/
  schemas/
    race.schema.json · class.schema.json · common.schema.json (source, bonus, trait)
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
| `Race` | `id`, `name`, `size`, `speed`, `bonuses[]`, `traits[]`, `languages[]`, `subraces[]`, `source` |
| `Subrace` | igual a `Race` sem `subraces`; somado à raça-mãe |
| `Class` | `id`, `name`, `hitDie`, `savingThrows[2]`, `skillChoices {count, from[]}`, `armor/weapon/tool proficiencies`, `features[]`, `source` |
| `ClassFeature` | `id`, `name`, `level`, `kind`, `description`, `grants[]` (ex.: `expertise`, `half-proficiency`), `source` |
| `Bonus` | `target` (`ability:x`, `skill:x`, `save:x`, `speed`, `hp-per-level`, `ac`), `value`, `bonusType`, `source` |
| `Trait` | `id`, `name`, `kind` (`sense`, `resistance`, `advantage`, `proficiency`, `language`, `spell`, `other`), `description`, `source` |

## Escolhas do jogador
Bônus que exigem escolha (ex.: Meio-Elfo +1 em dois atributos; Versatilidade em Perícia)
são modelados como `choice { count, from[], bonusType }`. A API devolve `warnings`
enquanto a escolha não for feita.

## Escopo de subclasses
MVP: só características **da classe base** e o marco "escolha seu arquétipo no nível N".
Detalhar arquétipos é fase 2 (ver `ROADMAP.md`), conforme licença (ADR-0004).

## Integridade (testada em `Ruleset.Tests`)
- Todo JSON valida no schema; `id` único por pasta; referências (`subraces`, `skill:x`) existem.
- Toda classe tem exatamente 2 salvaguardas e ≥ 1 feature no nível 1.
- Todo `source.page` está dentro do intervalo do capítulo correspondente.
