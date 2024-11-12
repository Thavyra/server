namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds all tokens for the specified authorization.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Token"/></returns>
public record Token_GetByAuthorization
{
    public required Guid AuthorizationId { get; init; }
}