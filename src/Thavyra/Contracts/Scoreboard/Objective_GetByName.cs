namespace Thavyra.Contracts.Scoreboard;

public record Objective_GetByName
{
    public required Guid ApplicationId { get; init; }
    public required string Name { get; init; }
}