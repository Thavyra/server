namespace Thavyra.Contracts.Application;

/// <summary>
/// Adds a new redirect for the specified application.
/// </summary>
/// <returns><see cref="Redirect"/></returns>
public record Redirect_Create
{
    public required Guid ApplicationId { get; init; }
    public required string Uri { get; init; }
}