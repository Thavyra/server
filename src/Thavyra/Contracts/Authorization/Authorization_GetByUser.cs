namespace Thavyra.Contracts.Authorization;

public record Authorization_GetByUser
{
    public required string UserId { get; init; }
}