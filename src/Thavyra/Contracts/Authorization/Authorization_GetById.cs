namespace Thavyra.Contracts.Authorization;

public record Authorization_GetById
{
    public required string Id { get; init; }
}