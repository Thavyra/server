namespace Thavyra.Contracts.Application;

/// <summary>
/// Deletes the specified application.
/// </summary>
public record Application_Delete
{
    public required Guid Id { get; init; }
}