namespace Thavyra.Contracts.Scoreboard;

public record Objective_Update
{
    public required Guid Id { get; init; }
    public Change<string> Name { get; init; }
    public Change<string> DisplayName { get; init; }
}