using Thavyra.Rest.Features.Objectives;
using Thavyra.Rest.Features.Scores;

namespace Thavyra.Rest.Features.Scoreboard.Objectives.Get;

public class Response : ObjectiveResponse
{
    public required List<ScoreResponseWithoutObjective> Scores { get; set; }
}