namespace Thavyra.Contracts.Authorization;

public record Authorization_GetByApplication
{
    public required string ApplicationId { get; init; }
}