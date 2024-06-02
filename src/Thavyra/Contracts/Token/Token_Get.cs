namespace Thavyra.Contracts.Token;

public record Token_Get
{
    public required string UserId { get; init; }
    public required string ApplicationId { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
}