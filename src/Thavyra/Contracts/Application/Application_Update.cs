namespace Thavyra.Contracts.Application;

/// <summary>
/// Updates the specified properties of the specified application.
/// </summary>
/// <returns><see cref="Application"/>, <see cref="NotFound"/></returns>
public record Application_Update
{
    public required Guid Id { get; init; }
    
    public Change<string> Name { get; set; }
    public Change<string?> Description { get; set; }
}