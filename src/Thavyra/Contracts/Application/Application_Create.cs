namespace Thavyra.Contracts.Application;

public record Application_Create
{
    public required Guid OwnerId { get; init; }
    public required string Name { get; init; }
}