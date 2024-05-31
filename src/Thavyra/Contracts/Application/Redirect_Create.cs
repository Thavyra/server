namespace Thavyra.Contracts.Application;

public record Redirect_Create
{
    public required string ApplicationId { get; init; }
    public required Uri Uri { get; init; }
}