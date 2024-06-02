namespace Thavyra.Contracts.Authorization;

public record Authorization_Delete
{
    public required Guid Id { get; init; }
}