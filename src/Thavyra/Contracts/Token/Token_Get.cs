namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds all tokens with the specified properties. Any set to null are ignored.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Token"/></returns>
public record Token_Get
{
    public required Guid UserId { get; init; }
    public required Guid ApplicationId { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
}