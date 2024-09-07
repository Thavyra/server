namespace Thavyra.Contracts.Application;

/// <summary>
/// Finds an application using its client ID.
/// </summary>
/// <returns><see cref="Application"/>, <see cref="NotFound"/></returns>
public record Application_GetByClientId
{
    public required string ClientId { get; init; }
}