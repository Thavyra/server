namespace Thavyra.Rest.Features.Scores;

public class ScoreResponseWithoutObjective
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required double Score { get; set; }
    public required DateTime CreatedAt { get; set; }
}