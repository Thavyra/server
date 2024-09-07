namespace Thavyra.Contracts.Token;

/// <summary>
/// Finds a token using the specified reference ID.
/// </summary>
/// <returns><see cref="Token"/>, <see cref="NotFound"/></returns>
public record Token_GetByReferenceId
{
    public required string ReferenceId { get; init; }
}