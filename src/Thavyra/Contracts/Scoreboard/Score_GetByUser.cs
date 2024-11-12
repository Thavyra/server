namespace Thavyra.Contracts.Scoreboard;

public record Score_GetByUser
{
    public required Guid UserId { get; init; }
}