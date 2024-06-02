namespace Thavyra.Contracts.Application;

public record Redirect
{
    public required Guid Id { get; init; }
    public required Guid ApplicationId { get; init; }

    public required string Uri { get; init; }

    public required DateTime CreatedAt { get; init; }
}