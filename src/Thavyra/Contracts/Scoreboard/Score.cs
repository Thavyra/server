namespace Thavyra.Contracts.Scoreboard;

public record Score
{
    public required Guid Id { get; init; }
    public required Guid ObjectiveId { get; init; }
    public required Guid UserId { get; init; }

    public required double Value { get; init; }

    public required DateTime CreatedAt { get; init; }
}