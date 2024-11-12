namespace Thavyra.Contracts.Scoreboard;

public record Objective_GetByApplication
{
    public required Guid ApplicationId { get; init; }   
}