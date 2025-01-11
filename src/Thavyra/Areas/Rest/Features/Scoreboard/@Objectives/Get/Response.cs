namespace Thavyra.Rest.Features.Scoreboard.Objectives.Get;

public class Response : ScoreboardObjectiveResponse
{
    public required List<ScoreResponse> Scores { get; set; }
}