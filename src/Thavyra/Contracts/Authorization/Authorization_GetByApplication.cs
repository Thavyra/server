namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Finds all authorizations for the specified application.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Authorization"/></returns>
public record Authorization_GetByApplication
{
    public required string ApplicationId { get; init; }
}