namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Lists all authorizations within the specified count and offset.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Authorization"/></returns>
public record Authorization_List
{
    public int? Count { get; init; }
    public int? Offset { get; init; }
}