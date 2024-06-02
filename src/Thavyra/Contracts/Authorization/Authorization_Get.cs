namespace Thavyra.Contracts.Authorization;

public record Authorization_Get
{
    public required string UserId { get; init; }
    public required string ApplicationId { get; init; }
    public string? Status { get; init; }
    public string? Type { get; set; }
}