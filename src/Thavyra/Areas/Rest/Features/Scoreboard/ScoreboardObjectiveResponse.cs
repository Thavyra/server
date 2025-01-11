namespace Thavyra.Rest.Features.Scoreboard;

public class ScoreboardObjectiveResponse
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }
    public required string DisplayName { get; set; }

    public required DateTime CreatedAt { get; set; }
}