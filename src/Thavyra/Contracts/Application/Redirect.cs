namespace Thavyra.Contracts.Application;

public record Redirect
{
    public required string Id { get; init; }
    public required string ApplicationId { get; init; }

    public required Uri Uri { get; init; }
}