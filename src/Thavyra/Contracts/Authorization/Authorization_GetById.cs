namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Finds an authorization using the specified ID.
/// </summary>
/// <returns><see cref="Authorization"/>, <see cref="NotFound"/></returns>
public record Authorization_GetById
{
    public required Guid Id { get; init; }
}