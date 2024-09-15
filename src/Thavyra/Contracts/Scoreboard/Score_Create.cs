namespace Thavyra.Contracts.Scoreboard;

public record Score_Create
{
    public required Guid ObjectiveId { get; init; }
    public required Guid UserId { get; init; }
    
    public required double Value { get; init; }
}