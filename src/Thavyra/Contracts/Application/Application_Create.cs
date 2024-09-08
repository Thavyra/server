namespace Thavyra.Contracts.Application;

/// <summary>
/// Creates a new OIDC application.
/// </summary>
/// <returns><see cref="Application"/></returns>
public record Application_Create
{
    public required Guid OwnerId { get; init; }

    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }

    public required string ConsentType { get; init; }
}