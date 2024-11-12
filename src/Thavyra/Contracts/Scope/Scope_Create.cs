namespace Thavyra.Contracts.Scope;

/// <summary>
/// Creates a new authorization scope.
/// </summary>
/// <returns><see cref="Scope"/></returns>
public record Scope_Create
{
    public Guid? Id { get; init; }

    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
}