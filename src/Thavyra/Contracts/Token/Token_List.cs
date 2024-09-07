namespace Thavyra.Contracts.Token;

/// <summary>
/// Lists all tokens within the specified count and offset.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Token"/></returns>
public record Token_List
{
    public int? Count { get; init; }
    public int? Offset { get; init; }
}