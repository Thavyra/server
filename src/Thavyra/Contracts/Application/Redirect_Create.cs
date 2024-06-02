namespace Thavyra.Contracts.Application;

public record Redirect_Create
{
    public required Guid ApplicationId { get; init; }
    public required string Uri { get; init; }
}