namespace DndBuilder.Domain.Tests;

/// <summary>F4 AC3: Total == ΣParts para todo valor derivado (casos pseudoaleatórios, semente fixa).</summary>
public class DerivedValueInvariantTests
{
    public static TheoryData<int> Seeds() => [.. Enumerable.Range(1, 50)];

    [Theory]
    [MemberData(nameof(Seeds))]
    public void AllRules_RandomInputs_TotalEqualsSumOfParts(int seed)
    {
        var rng = new Random(seed);
        var baseScore = rng.Next(1, 21);
        var score = new AbilityScore(rng.Next(1, 31));
        var level = new Level(rng.Next(1, 21));
        var skill = (Skill)rng.Next(Enum.GetValues<Skill>().Length);
        var proficiency = (SkillProficiency)rng.Next(4);
        var increases = Enumerable.Range(0, rng.Next(0, 3))
            .Select(_ => new BonusPart(rng.Next(1, 3), "Raça", BonusType.AbilityScoreIncrease, SourceRef.LdJ(20)));
        var skillValue = SkillRules.Compute(skill, score, proficiency, level);

        DerivedValue[] values =
        [
            AbilityScoreRules.FinalScore(baseScore, increases),
            AbilityScoreRules.Modifier(Ability.Forca, score),
            ProficiencyBonus.Derive(level),
            SavingThrowRules.Compute(Ability.Sabedoria, score, rng.Next(2) == 1, level),
            skillValue,
            PassivePerceptionRules.Compute(skillValue),
        ];

        Assert.All(values, v => Assert.Equal(v.Parts.Sum(p => p.Value), v.Total));
    }
}
