namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Updates the specified authorization with the specified properties.
/// </summary>
/// <returns><see cref="Authorization"/>, <see cref="NotFound"/></returns>
public record Authorization_Update
{
    public required Guid Id { get; init; }
    
    public Change<string?> Type { get; init; }
    public Change<string?> Status { get; init; }
}