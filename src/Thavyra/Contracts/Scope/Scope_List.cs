namespace Thavyra.Contracts.Scope;

/// <summary>
/// Lists all scopes within the specified count and offset.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Scope"/></returns>
public record Scope_List
{
    public int? Count { get; init; }
    public int? Offset { get; init; }
}