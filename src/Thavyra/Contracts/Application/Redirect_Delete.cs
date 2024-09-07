namespace Thavyra.Contracts.Application;

/// <summary>
/// Deletes the specified redirect.
/// </summary>
public record Redirect_Delete
{
    public required Guid Id { get; init; }  
}