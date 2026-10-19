# Tipos de bônus (critério 4)

Todo bônus exibido tem **origem** (raça, sub-raça, classe, nível, atributo, escolha)
e **tipo** (abaixo). O `bonusType` é um enum fechado no schema e no `Domain`.

## Bônus numéricos (entram em `parts[]` de um `DerivedValue`)
| `bonusType` | Nome na UI | Exemplo | Origem típica |
|---|---|---|---|
| `base-score` | Valor base | Destreza 15 (conjunto padrão) | Escolha do jogador |
| `ability-score-increase` | Aumento de atributo | Anão: +2 Constituição | Raça / sub-raça |
| `ability-modifier` | Modificador de atributo | +3 de Destreza 16 | Atributo |
| `proficiency-bonus` | Bônus de proficiência | +2 no nível 1 | Nível |
| `proficiency` | Proficiência | Furtividade proficiente → +2 | Classe / raça / antecedente |
| `expertise` | Especialização | Furtividade → +4 | Característica de classe |
| `half-proficiency` | Versatilidade | +1 em perícias sem treino | Bardo nível 2 |
| `hit-points` | Pontos de vida extra | +1 PV/nível | Anão da Colina |
| `speed` | Deslocamento | 7,5 m (Anão) | Raça |
| `armor-class` | Classe de Armadura | Defesa sem Armadura | Classe |

## Benefícios não numéricos (exibidos como traços/características)
| `kind` | Nome na UI | Exemplo |
|---|---|---|
| `sense` | Sentido | Visão no Escuro 18 m |
| `resistance` | Resistência a dano | Resiliência Anã (veneno) |
| `advantage` | Vantagem | Vantagem em salvaguardas contra veneno |
| `proficiency` | Proficiência (arma/armadura/ferramenta) | Treinamento Anão em Combate |
| `language` | Idioma | Anão, Comum |
| `spell` | Magia inata | Legado Infernal (Tiefling) |
| `other` | Outro | Ataque Furtivo, Fúria |

## Legenda visual (UI)
- Cada `bonusType` tem **cor + ícone + rótulo** (nunca só cor — acessibilidade).
- O popover `bonus-breakdown` lista as parcelas na ordem: base → atributo → proficiência → extras,
  com a página do LdJ em cada linha.
- "Vantagem" é explicada como **rolar 2d20 e ficar com o maior**, não como número.
