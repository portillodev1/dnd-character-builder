# rules/backend-dotnet.md — carregada em: código .NET (Api, Application, Domain, Infrastructure)

Stack: **.NET 10 LTS**, ASP.NET Core **Minimal APIs**, solução `backend/DndBuilder.slnx`.
Arquitetura detalhada (humanos): `docs/architecture/backend.md`.

## Dependências entre projetos (verificado por teste de arquitetura)
```txt
Api ──► Application ──► Domain
 └────► Infrastructure ──► Application, Domain
Domain: ZERO dependências (nem ASP.NET, nem System.Text.Json attributes)
```

## Regras
- `Domain` é puro e determinístico: sem I/O, sem DI, sem `DateTime.Now`. Só tipos e funções.
- Valores do jogo são **value objects** imutáveis (`record struct`): `AbilityScore` (1–30),
  `Level` (1–20), `ProficiencyBonus`, `Modifier`. Construtor rejeita valor inválido.
- Todo resultado derivado é `DerivedValue { Total, IReadOnlyList<BonusPart> Parts }`;
  `Total == Parts.Sum(p => p.Value)` é invariante (teste obrigatório).
- `Application` expõe casos de uso (1 classe/handler por caso de uso) e portas
  (`IRulesetCatalog`). Sem MediatR — chamada direta.
- `Infrastructure` implementa portas: carrega `content/` como **embedded resource**,
  valida contra JSON Schema no startup (falha rápida).
- `Api`: endpoints agrupados por recurso (`MapGroup("/api/v1/races")`), DTOs próprios
  (nunca expor tipo do Domain), erros como **ProblemDetails** (RFC 9457).
- OpenAPI via `Microsoft.AspNetCore.OpenApi` (nativo); documento em `/openapi/v1.json`.
- Nullable e `TreatWarningsAsErrors` ligados em `Directory.Build.props`.
- Versões de pacotes centralizadas em `Directory.Packages.props` (CPM).

## Exemplo — value object com invariante
```csharp
public readonly record struct AbilityScore
{
    public int Value { get; }
    public AbilityScore(int value) => Value = value is >= 1 and <= 30
        ? value : throw new ArgumentOutOfRangeException(nameof(value)); // LdJ p. 175
    public Modifier Modifier => new((int)Math.Floor((Value - 10) / 2.0));
}
```
