namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds all tokens for the specified application.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Token"/></returns>
public record Token_GetByApplication
{
    public required string ApplicationId { get; init; }
}