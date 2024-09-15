namespace Thavyra.Contracts.Scoreboard;

public record Objective
{
    public required Guid Id { get; init; }
    public required Guid ApplicationId { get; init; }

    public required string Name { get; init; }

    public required IReadOnlyList<Score> Scores { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}