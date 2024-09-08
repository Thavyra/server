namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds a token using the specified ID.
/// </summary>
/// <returns><see cref="Token"/>, <see cref="NotFound"/></returns>
public record Token_GetById
{
    public required Guid Id { get; init; }
}