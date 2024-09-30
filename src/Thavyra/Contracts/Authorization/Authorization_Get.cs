namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Finds all authorizations using the specified properties. Null properties are ignored.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Authorization"/></returns>
public record Authorization_Get
{
    public required Guid Subject { get; init; }
    public required Guid ApplicationId { get; init; }
    public string? Status { get; init; }
    public string? Type { get; set; }
}