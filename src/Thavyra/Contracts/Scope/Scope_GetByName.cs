namespace Thavyra.Contracts.Scope;

/// <summary>
/// Finds a scope using the specified name.
/// </summary>
/// <returns><see cref="Scope"/>, <see cref="NotFound"/></returns>
public record Scope_GetByName
{
    public required string Name { get; init; }
}