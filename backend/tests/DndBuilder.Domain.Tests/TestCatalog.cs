using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain.Tests;

/// <summary>Recorte do Anão e do Ladino em memória (sem disco — rules/tests.md).</summary>
internal static class TestCatalog
{
    private static SourceRef P(int page) => SourceRef.LdJ(page);

    public static readonly Race Anao = new()
    {
        Id = "anao",
        Name = "Anão",
        Summary = "s",
        Size = CreatureSize.Medio,
        Speed = new Speed { Meters = 7.5m, Source = P(20) },
        Bonuses = [new CatalogBonus { Target = "ability:constituicao", Value = 2, BonusType = BonusType.AbilityScoreIncrease, Label = "Anão", Source = P(20) }],
        Choices = [new Choice { Id = "ferramentas", Name = "Proficiência com Ferramentas", Count = 1, From = ["a", "b"], Source = P(20) }],
        Traits = [new Trait { Id = "visao-no-escuro", Name = "Visão no Escuro", Kind = TraitKind.Sense, Description = "d", Source = P(20) }],
        Languages = ["Comum", "Anão"],
        Subraces =
        [
            new Subrace
            {
                Id = "anao-da-colina", Name = "Anão da Colina", Summary = "s", Source = P(20),
                Bonuses =
                [
                    new CatalogBonus { Target = "ability:sabedoria", Value = 1, BonusType = BonusType.AbilityScoreIncrease, Source = P(20) },
                    new CatalogBonus { Target = "hp-per-level", Value = 1, BonusType = BonusType.HitPoints, Label = "Tenacidade Anã", Source = P(20) },
                ],
                Traits = [new Trait { Id = "tenacidade-ana", Name = "Tenacidade Anã", Kind = TraitKind.Other, Description = "d", Source = P(20) }],
            },
        ],
        Source = P(18),
    };

    /// <summary>Raça fictícia com perícia racial fixa (como o Elfo → Percepção).</summary>
    public static readonly Race Elfo = new()
    {
        Id = "elfo",
        Name = "Elfo",
        Summary = "s",
        Size = CreatureSize.Medio,
        Speed = new Speed { Meters = 9m, Source = P(23) },
        Traits = [new Trait { Id = "sentidos-agucados", Name = "Sentidos Aguçados", Kind = TraitKind.Proficiency, Description = "d",
            Grants = [new Grant { Type = GrantType.SkillProficiency, Skill = "percepcao" }], Source = P(23) }],
        Languages = ["Comum"],
        Subraces = [new Subrace { Id = "elfo-da-floresta", Name = "Elfo da Floresta", Summary = "s", Source = P(24),
            Speed = new Speed { Meters = 10.5m, Label = "Pés Ligeiros", Source = P(24) } }],
        Source = P(21),
    };

    /// <summary>Raça fictícia com escolhas que entram no cálculo (como o Meio-Elfo).</summary>
    public static readonly Race MeioElfo = new()
    {
        Id = "meio-elfo",
        Name = "Meio-Elfo",
        Summary = "s",
        Size = CreatureSize.Medio,
        Speed = new Speed { Meters = 9m, Source = P(39) },
        Bonuses = [new CatalogBonus { Target = "ability:carisma", Value = 2, BonusType = BonusType.AbilityScoreIncrease, Source = P(39) }],
        Choices =
        [
            new Choice { Id = "aumento-de-atributo", Name = "Aumento de Atributo", Count = 2, Value = 1, BonusType = BonusType.AbilityScoreIncrease,
                From = ["ability:forca", "ability:destreza", "ability:sabedoria"], Source = P(39) },
            new Choice { Id = "versatilidade-em-pericia", Name = "Versatilidade em Perícia", Count = 2, BonusType = BonusType.Proficiency,
                From = ["skill:atletismo", "skill:historia", "skill:medicina"], Source = P(39) },
        ],
        Languages = ["Comum", "Élfico"],
        Source = P(38),
    };

    private static ClassFeature F(string id, int level, params Grant[] grants)
        => new() { Id = id, Name = id, Level = level, Kind = TraitKind.Other, Description = "d", Grants = grants, Source = P(91) };

    public static readonly CharacterClass Ladino = new()
    {
        Id = "ladino",
        Name = "Ladino",
        Summary = "s",
        HitDie = 8,
        PrimaryAbilities = [Ability.Destreza],
        SavingThrows = [Ability.Destreza, Ability.Inteligencia],
        SkillChoices = new SkillChoices { Count = 4, From = ["acrobacia", "atletismo", "enganacao", "furtividade", "intuicao", "percepcao", "investigacao"] },
        Proficiencies = new ClassProficiencies(),
        SubclassLevel = 3,
        Table = [new ScalingColumn { Id = "ataque-furtivo", Name = "Ataque Furtivo", Values = [.. Enumerable.Range(1, 20).Select(n => $"{(n + 1) / 2}d6")] }],
        Features =
        [
            F("especializacao", 1, new Grant { Type = GrantType.Expertise, Count = 2 }),
            F("ataque-furtivo", 1) with { Scaling = "ataque-furtivo" },
            F("acao-ardilosa", 2),
            F("arquetipo-de-ladino", 3, new Grant { Type = GrantType.SubclassChoice }),
            F("incremento-no-valor-de-habilidade-4", 4, new Grant { Type = GrantType.AbilityScoreImprovement }),
            F("especializacao-6", 6, new Grant { Type = GrantType.Expertise, Count = 2 }),
            F("incremento-no-valor-de-habilidade-8", 8, new Grant { Type = GrantType.AbilityScoreImprovement }),
            F("mente-escorregadia", 15, new Grant { Type = GrantType.SaveProficiency, Ability = "sabedoria" }),
            F("golpe-de-sorte", 20),
        ],
        Source = P(89),
    };

    private static CharacterClass Marcial(string id, string name, int page, params ClassFeature[] features) => new()
    {
        Id = id,
        Name = name,
        Summary = "s",
        HitDie = 12,
        PrimaryAbilities = [Ability.Forca],
        SavingThrows = [Ability.Forca, Ability.Constituicao],
        SkillChoices = new SkillChoices { Count = 2, From = ["atletismo", "percepcao"] },
        Proficiencies = new ClassProficiencies(),
        SubclassLevel = 3,
        Features = features,
        Source = P(page),
    };

    /// <summary>Bárbaro: Movimento Rápido +3 m a partir do nível 5, sem armadura pesada (LdJ p. 49).</summary>
    public static readonly CharacterClass Barbaro = Marcial("barbaro", "Bárbaro", 46,
        F("movimento-rapido", 5, new Grant { Type = GrantType.SpeedBonus, Meters = 3m, Condition = SpeedCondition.NoHeavyArmor })
            with
        { Name = "Movimento Rápido", Source = P(49) });

    /// <summary>Monge: Movimento sem Armadura por degraus, nível 2/6/10/14/18 (LdJ p. 104).</summary>
    public static readonly CharacterClass Monge = Marcial("monge", "Monge", 102,
        F("movimento-sem-armadura", 2, new Grant
        {
            Type = GrantType.SpeedBonus,
            Condition = SpeedCondition.NoArmorNoShield,
            Steps = [new(2, 3m), new(6, 4.5m), new(10, 6m), new(14, 7.5m), new(18, 9m)],
        }) with
        { Name = "Movimento sem Armadura", Source = P(104) });

    /// <summary>Guerreiro fictício com Campeão (LdJ p. 83–86): marco no nível 3, rótulos genéricos nos níveis 7 e 10.</summary>
    public static readonly CharacterClass Guerreiro = Marcial("guerreiro", "Guerreiro", 83,
        F("arquetipo-marcial", 3, new Grant { Type = GrantType.SubclassChoice }) with { Name = "Arquétipo Marcial" },
        F("caracteristica-de-arquetipo-marcial-7", 7, new Grant { Type = GrantType.ArchetypeSlot }) with { Name = "Característica de Arquétipo Marcial" },
        F("caracteristica-de-arquetipo-marcial-10", 10, new Grant { Type = GrantType.ArchetypeSlot }) with { Name = "Característica de Arquétipo Marcial" }) with
    {
        HitDie = 10,
        Archetypes =
        [
            new Archetype
            {
                Id = "campeao",
                ClassId = "guerreiro",
                Name = "Campeão",
                Summary = "s",
                Srd = true,
                Features = [F("critico-aprimorado", 3) with { Name = "Crítico Aprimorado" }, F("atletismo-extraordinario", 7) with { Name = "Atletismo Extraordinário" }],
                Source = P(85),
            },
        ],
    };

    /// <summary>Feiticeiro fictício com Linhagem Dracônica (p. 77–80): marco no nível 1, +1 PV por nível e escolha do ancestral.</summary>
    public static readonly CharacterClass Feiticeiro = Marcial("feiticeiro", "Feiticeiro", 77,
        F("origem-de-feiticaria", 1, new Grant { Type = GrantType.SubclassChoice }) with { Name = "Origem de Feitiçaria" },
        F("caracteristica-de-origem-de-feiticaria-6", 6, new Grant { Type = GrantType.ArchetypeSlot }) with { Name = "Característica de Origem de Feitiçaria" }) with
    {
        HitDie = 6,
        SubclassLevel = 1,
        Archetypes =
        [
            new Archetype
            {
                Id = "linhagem-draconica",
                ClassId = "feiticeiro",
                Name = "Linhagem Dracônica",
                Summary = "s",
                Srd = true,
                Bonuses = [new CatalogBonus { Target = "hp-per-level", Value = 1, BonusType = BonusType.HitPoints, Label = "Resiliência Dracônica", Source = P(80) }],
                Choices = [new Choice { Id = "linhagem-draconica-ancestral", Name = "Ancestral Dracônico", Count = 1, From = ["Azul", "Vermelho"], Kind = TraitKind.Other, Source = P(80) }],
                Features = [F("resiliencia-draconica", 1) with { Name = "Resiliência Dracônica" }, F("afinidade-elemental", 6) with { Name = "Afinidade Elemental" }],
                Source = P(80),
            },
        ],
    };

    public static readonly Race[] Races = [Anao, Elfo, MeioElfo];
    public static readonly CharacterClass[] Classes = [Ladino, Barbaro, Monge, Guerreiro, Feiticeiro];
}
