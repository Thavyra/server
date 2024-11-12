namespace Thavyra.Contracts.Scoreboard;

public record Objective_GetById
{
    public required Guid Id { get; init; }
}