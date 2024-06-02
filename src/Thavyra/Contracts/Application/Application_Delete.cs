namespace Thavyra.Contracts.Application;

public record Application_Delete
{
    public required Guid Id { get; init; }
}