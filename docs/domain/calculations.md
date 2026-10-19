# Fórmulas de cálculo (fonte para o `Domain`)

Cada fórmula vira teste de tabela em `DndBuilder.Domain.Tests` (ver `rules/tests.md`).

## 1. Valor de atributo (LdJ p. 12–13, 175)
```txt
valorFinal = valorBase + Σ(aumentos raciais e sub-raciais) [+ ASI de nível, fase 2]
teto: 20 para jogadores (30 é o máximo absoluto do sistema)
```
Métodos de valor base (Cap. 1): **conjunto padrão** 15,14,13,12,10,8 · **compra de pontos**
(27 pontos, 8–15 antes de bônus) · **manual** (rolagem 4d6, descarta o menor).

## 2. Modificador (LdJ p. 175)
```txt
mod = ⌊(valor − 10) / 2⌋     →  1:−5 · 8–9:−1 · 10–11:0 · 12–13:+1 · 20:+5 · 30:+10
```

## 3. Bônus de proficiência por nível (LdJ p. 15)
| Nível | 1–4 | 5–8 | 9–12 | 13–16 | 17–20 |
|---|---|---|---|---|---|
| Bônus | +2 | +3 | +4 | +5 | +6 |
Equivalente: `2 + ⌊(nível − 1) / 4⌋`.

## 4. Salvaguarda (LdJ p. 181)
```txt
salvaguarda(atr) = mod(atr) + (proficiente(atr) ? bônusProf : 0)
```
Proficiência vem da **classe inicial** (2 atributos, ver glossário).

## 5. Perícia (LdJ p. 174–176)
```txt
perícia = mod(atributoChave) + fator × bônusProf
fator: 0 sem proficiência · ½ (arred. p/ baixo) Versatilidade do Bardo
       1 proficiente · 2 Especialização (Ladino nív. 1 e 6; Bardo nív. 3 e 10)
```
Fontes de proficiência: classe (escolha da lista), antecedente (2), raça
(ex.: Elfo → Percepção; Meio-Orc → Intimidação; Meio-Elfo → 2 à escolha).
Proficiência duplicada não acumula — o wizard pede outra escolha.

## 6. Sabedoria Passiva (Percepção) (LdJ p. 175)
```txt
passiva = 10 + perícia(Percepção)
```

## 7. Pontos de vida (fora dos critérios, exibido) (Cap. 3)
```txt
nível 1: dadoDeVida máx + mod(Con)
nível n: + (média do dado arred. p/ cima + mod(Con)) por nível
extras: Anão da Colina +1 por nível (Robustez Anã)
```

## Breakdown obrigatório
Toda fórmula devolve `DerivedValue { total, parts[] }`; cada parcela tem `bonusType`
(ver [bonus-types.md](bonus-types.md)) e `source` (página do LdJ).
