namespace Thavyra.Contracts.Scoreboard;

public record Objective_ExistsByName
{
    public required Guid ApplicationId { get; init; }
    public required string Name { get; init; }
};