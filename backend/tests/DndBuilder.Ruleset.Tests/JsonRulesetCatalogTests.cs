using DndBuilder.Domain.Catalog;
using DndBuilder.Infrastructure.Content;

namespace DndBuilder.Ruleset.Tests;

public class JsonRulesetCatalogTests
{
    private static readonly JsonRulesetCatalog Catalog = JsonRulesetCatalog.Load();

    [Fact] // JSON → Domain: enums em kebab-case, sub-raças e escolhas
    public void Load_Dwarf_MapsSubracesBonusesAndChoices()
    {
        var anao = Catalog.Races.Single(r => r.Id == "anao");
        Assert.Equal(("anao-da-colina anao-da-montanha", 7.5m, 1), (string.Join(' ', anao.Subraces.Select(s => s.Id)), anao.Speed.Meters, anao.Choices.Count));
    }

    [Fact]
    public void Load_Rogue_MapsFeaturesTableAndGrants()
    {
        var ladino = Catalog.Classes.Single(c => c.Id == "ladino");
        Assert.Equal((8, 22, 2, "10d6"), (ladino.HitDie, ladino.Features.Count, ladino.Features[0].Grants[0].Count, ladino.Table[0].Values[^1]));
    }

    [Fact] // F11 AC4: Bárbaro +3 m fixo (LdJ p. 49) e Monge por degraus (p. 104)
    public void Load_BarbarianAndMonk_MapSpeedBonusGrants()
    {
        var barbaro = Catalog.Classes.Single(c => c.Id == "barbaro").Features.Single(f => f.Id == "movimento-rapido").Grants.Single();
        var monge = Catalog.Classes.Single(c => c.Id == "monge").Features.Single(f => f.Id == "movimento-sem-armadura").Grants.Single();
        Assert.Equal(
            (GrantType.SpeedBonus, 3m, SpeedCondition.NoHeavyArmor, GrantType.SpeedBonus, SpeedCondition.NoArmorNoShield, "2:3 6:4.5 10:6 14:7.5 18:9"),
            (barbaro.Type, barbaro.Meters, barbaro.Condition, monge.Type, monge.Condition,
                string.Join(' ', monge.Steps!.Select(s => FormattableString.Invariant($"{s.Level}:{s.Meters}")))));
    }

    [Fact] // F11: os degraus do Monge repetem a coluna "Movimento sem Armadura" da tabela da classe, nível a nível
    public void MongeSpeedSteps_AgainstClassTable_MatchEveryLevel()
    {
        var monge = Catalog.Classes.Single(c => c.Id == "monge");
        var steps = monge.Features.Single(f => f.Id == "movimento-sem-armadura").Grants.Single().Steps!;
        var coluna = monge.Table.Single(t => t.Id == "movimento-sem-armadura").Values;
        var daTabela = coluna.Select(v => v == "—" ? 0m : decimal.Parse(v.Replace("+", "").Replace(" m", "").Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture));
        var dosDegraus = Enumerable.Range(1, 20).Select(n => steps.LastOrDefault(s => s.Level <= n)?.Meters ?? 0m);
        Assert.Equal(daTabela, dosDegraus);
    }

    [Fact] // falha rápida (ADR-0003): conteúdo inválido não carrega
    public void Load_InvalidContent_Throws()
    {
        var files = Infrastructure.Content.EmbeddedContent.ReadAll().ToDictionary(f => f.Key, f => f.Value);
        files["classes/ladino.json"] = files["classes/ladino.json"].Replace("\"hitDie\": 8", "\"hitDie\": 7", StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => JsonRulesetCatalog.Load(files));
    }
}
