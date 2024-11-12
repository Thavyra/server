namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Deletes the specified authorization.
/// </summary>
public record Authorization_Delete
{
    public required Guid Id { get; init; }
}