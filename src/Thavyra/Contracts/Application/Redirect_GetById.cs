namespace Thavyra.Contracts.Application;

public record Redirect_GetById
{
    public required Guid Id { get; init; }
    public required Guid ApplicationId { get; init; }
}