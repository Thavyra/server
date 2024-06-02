namespace Thavyra.Contracts.Application;

public record Application_GetByRedirect
{
    public required string Uri { get; init; }
}