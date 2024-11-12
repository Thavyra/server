namespace Thavyra.Contracts.Scope;

/// <summary>
/// Finds all scopes using the specified list of names.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Scope"/></returns>
public record Scope_GetByNames
{
    public required List<string> Names { get; init; }
}