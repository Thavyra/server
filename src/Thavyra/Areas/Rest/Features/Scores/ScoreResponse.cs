using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Scores;

public class ScoreResponse : ScoreResponseWithoutObjective
{
    public required Guid ObjectiveId { get; set; }
    
}