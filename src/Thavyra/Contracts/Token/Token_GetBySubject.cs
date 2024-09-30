namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds all tokens for the specified user.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Token"/></returns>
public record Token_GetBySubject
{
    public required Guid Subject { get; init; }
}