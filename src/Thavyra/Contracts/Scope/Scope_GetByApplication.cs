namespace Thavyra.Contracts.Scope;

/// <summary>
/// Finds all scopes which the specified application is permitted to use.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Scope"/></returns>
public record Scope_GetByApplication
{
    public required Guid ApplicationId { get; init; }
}
