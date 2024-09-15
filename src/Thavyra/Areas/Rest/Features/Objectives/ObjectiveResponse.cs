namespace Thavyra.Rest.Features.Objectives;

public class ObjectiveResponse
{
    public required Guid Id { get; set; }
    public required Guid ApplicationId { get; set; }

    public required string Name { get; set; }

    public required DateTime CreatedAt { get; set; }
}