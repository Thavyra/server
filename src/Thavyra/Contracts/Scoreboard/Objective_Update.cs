namespace Thavyra.Contracts.Scoreboard;

public record Objective_Update
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}