# Fórmulas de cálculo (fonte para o `Domain`)

Cada fórmula vira teste de tabela em `DndBuilder.Domain.Tests` (ver `rules/tests.md`).

## 1. Valor de atributo (LdJ p. 13, 15, 175)
```txt
valorFinal = valorBase + Σ(aumentos raciais e sub-raciais) + Σ(Incrementos de nível, +1 cada)
teto: 20 para jogadores (30 é o máximo absoluto do sistema)
```
Ordem: raça → escolhas raciais → Incrementos em ordem de nível (spec F10); o teto corta parcela a
parcela, então o +1 que passaria de 20 aparece como 0 "(limitado ao teto de 20)".
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

## 5. Perícia (LdJ p. 176–181)
```txt
perícia = mod(atributoChave) + fator × bônusProf
fator: 0 sem proficiência · ½ (arred. p/ baixo) Versatilidade do Bardo
       1 proficiente · 2 Especialização (Ladino nív. 1 e 6; Bardo nív. 3 e 10)
```
Fontes de proficiência: classe (escolha da lista), antecedente (2), raça
(ex.: Elfo → Percepção; Meio-Orc → Intimidação; Meio-Elfo → 2 à escolha).
Proficiência duplicada não acumula — o wizard pede outra escolha.

## 6. Sabedoria Passiva (Percepção) (LdJ p. 177)
```txt
passiva = 10 + perícia(Percepção)
```

## 7. Pontos de vida (fora dos critérios, exibido) (Cap. 3)
```txt
nível 1: dadoDeVida máx + mod(Con)
nível n: + (média do dado arred. p/ cima + mod(Con)) por nível
extras: Anão da Colina +1 por nível (Tenacidade Anã)
```

## 8. Deslocamento (exibido) (LdJ Cap. 2)
```txt
deslocamento = deslocamento da raça (ex.: Anão 7,5 m p. 20; Elfo 9 m p. 23)
             + (deslocamento da sub-raça − da raça), se a sub-raça o redefine
               (Elfo da Floresta, Pés Ligeiros: 10,5 m, p. 24 → parcela +1,5 m)
             + bônus de classe até o nível (spec F11):
               Bárbaro nv. 5+: +3 m (Movimento Rápido, p. 49; sem armadura pesada)
               Monge nv. 2/6/10/14/18: +3 / +4,5 / +6 / +7,5 / +9 m (p. 104; sem armadura nem escudo)
```
A condição do livro só aparece no rótulo da parcela (o app não modela armadura).
Em metros, com decimais: sai como `DerivedMeasure` (mesma invariante `total = Σ parts`).

## Breakdown obrigatório
Toda fórmula devolve `DerivedValue { total, parts[] }`; cada parcela tem `bonusType`
(ver [bonus-types.md](bonus-types.md)) e `source` (página do LdJ).
