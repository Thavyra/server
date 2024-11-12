namespace Thavyra.Contracts.Scope;

/// <summary>
/// Updates the specified scope with the specified properties.
/// </summary>
/// <returns><see cref="Scope"/>, <see cref="NotFound"/></returns>
public record Scope_Update
{
    public required Guid Id { get; init; }

    public Change<string> DisplayName { get; init; }
    public Change<string> Description { get; init; }
}