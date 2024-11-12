namespace Thavyra.Contracts.Scoreboard;

public record Objective_Create
{
    public required Guid ApplicationId { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
}