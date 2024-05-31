namespace Thavyra.Contracts.Application;

public record Application_GetById
{
    public required string Id { get; init; }
}