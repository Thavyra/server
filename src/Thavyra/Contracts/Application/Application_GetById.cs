namespace Thavyra.Contracts.Application;

/// <summary>
/// Finds an application using its ID.
/// </summary>
/// <returns><see cref="Application"/>, <see cref="NotFound"/></returns>
public record Application_GetById
{
    public required string Id { get; init; }
}