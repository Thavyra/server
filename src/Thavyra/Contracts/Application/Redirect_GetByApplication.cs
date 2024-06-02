namespace Thavyra.Contracts.Application;

public record Redirect_GetByApplication
{
    public required Guid ApplicationId { get; init; }
}