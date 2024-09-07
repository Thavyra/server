namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Finds all authorizations for the specified user.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Authorization"/></returns>
public record Authorization_GetByUser
{
    public required string UserId { get; init; }
}