namespace Thavyra.Contracts.Token;

/// <summary>
/// Removes all tokens associated with the specified authorization.
/// </summary>
/// <returns><see cref="Count"/></returns>
public record Token_RevokeByAuthorization
{
    public required Guid AuthorizationId { get; init; }
}