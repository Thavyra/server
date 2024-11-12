namespace Thavyra.Contracts.Application;

/// <summary>
/// Finds all applications owned by the specified user.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Application"/></returns>
public record Application_GetByOwner
{
    public Guid OwnerId { get; init; }
}