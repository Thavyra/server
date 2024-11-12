namespace Thavyra.Contracts.Scope;

/// <summary>
/// Deletes the specified scope.
/// </summary>
public record Scope_Delete
{
    public required Guid Id { get; init; }
}