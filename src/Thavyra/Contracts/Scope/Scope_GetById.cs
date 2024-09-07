namespace Thavyra.Contracts.Scope;

/// <summary>
/// Finds a scope using the specified ID.
/// </summary>
/// <returns><see cref="Scope"/>, <see cref="NotFound"/></returns>
public record Scope_GetById
{
    public required string Id { get; init; }
}