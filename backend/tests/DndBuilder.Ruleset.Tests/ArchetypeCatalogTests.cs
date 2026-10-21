using DndBuilder.Domain;
using DndBuilder.Domain.Catalog;
using DndBuilder.Infrastructure.Content;

namespace DndBuilder.Ruleset.Tests;

/// <summary>Spec F12 (ADR-0011): arquétipos em <c>content/archetypes/</c> ligados às classes.</summary>
public class ArchetypeCatalogTests
{
    private static readonly JsonRulesetCatalog Loaded = JsonRulesetCatalog.Load();

    [Fact] // AC9: cada arquétipo chega à classe dona, com srd e bônus tipado
    public void Load_Archetypes_AttachToTheirClasses()
    {
        var campeao = Loaded.Classes.Single(c => c.Id == "guerreiro").Archetypes.Single();
        var dracônica = Loaded.Classes.Single(c => c.Id == "feiticeiro").Archetypes.Single();
        Assert.Equal(
            ("campeao", true, "3 7 10 15 18", "hp-per-level", BonusType.HitPoints, "linhagem-draconica-ancestral"),
            (campeao.Id, campeao.Srd, string.Join(' ', campeao.Features.Select(f => f.Level)),
                dracônica.Bonuses.Single().Target, dracônica.Bonuses.Single().BonusType, dracônica.Choices.Single().Id));
    }

    [Fact] // AC2: os rótulos genéricos da classe levam o grant archetype-slot (e só eles)
    public void ArchetypeSlot_Classes_MarkEveryGenericLabel()
    {
        var generic = Loaded.Classes.SelectMany(c => c.Features).Where(f => f.Name.StartsWith("Característica de ", StringComparison.Ordinal));
        var slots = Loaded.Classes.SelectMany(c => c.Features).Where(f => f.Grants.Any(g => g.Type == GrantType.ArchetypeSlot));
        Assert.Equal(generic.Select(f => f.Id).Order(), slots.Select(f => f.Id).Order());
    }

    [Fact] // AC9: níveis só os que a classe reserva ao arquétipo (marco + rótulos genéricos) e ≥ 1 característica no marco
    public void Features_EveryArchetype_UseOnlyLevelsTheClassReserves()
    {
        foreach (var @class in Loaded.Classes)
        {
            var reserved = @class.Features.Where(f => f.Grants.Any(g => g.Type == GrantType.ArchetypeSlot)).Select(f => f.Level)
                .Append(@class.SubclassLevel).ToHashSet();
            foreach (var archetype in @class.Archetypes)
            {
                Assert.True(archetype.Features.Any(f => f.Level == @class.SubclassLevel), $"{archetype.Id}: sem característica no marco");
                Assert.All(archetype.Features, f => Assert.Contains(f.Level, reserved));
            }
        }
    }

    [Fact] // escolhas de arquétipo dividem o mapa draft.choices com as da raça: ids não podem colidir
    public void ChoiceIds_Archetypes_AreUniqueAndDoNotCollideWithRaceChoices()
    {
        var archetype = Loaded.Classes.SelectMany(c => c.Archetypes).SelectMany(a => a.Choices).Select(c => c.Id).ToList();
        var race = Loaded.Races.SelectMany(r => r.Choices.Concat(r.Subraces.SelectMany(s => s.Choices))).Select(c => c.Id).ToHashSet();
        Assert.Equal((archetype.Count, 0), (archetype.Distinct().Count(), archetype.Count(race.Contains)));
    }

    [Fact] // falha rápida: classId inexistente não carrega
    public void Load_ArchetypeOfUnknownClass_Throws()
    {
        var files = EmbeddedContent.ReadAll().ToDictionary(f => f.Key, f => f.Value);
        files["archetypes/campeao.json"] = files["archetypes/campeao.json"].Replace("\"classId\": \"guerreiro\"", "\"classId\": \"gladiador\"", StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => JsonRulesetCatalog.Load(files));
    }

    [Fact] // schema: arquétipo sem srd ou com característica sem página é rejeitado
    public void Validate_ArchetypeWithoutSrdFlag_ReportsError()
    {
        var files = EmbeddedContent.ReadAll().ToDictionary(f => f.Key, f => f.Value);
        files["archetypes/campeao.json"] = files["archetypes/campeao.json"].Replace("\"srd\": true,", "", StringComparison.Ordinal);
        Assert.Contains(ContentValidator.Validate(files), e => e.Path == "archetypes/campeao.json");
    }
}
