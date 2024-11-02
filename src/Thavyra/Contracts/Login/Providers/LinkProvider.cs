namespace Thavyra.Contracts.Login.Providers;

public record LinkProvider
{
    public required Guid UserId { get; init; }
    public required string Provider { get; init; }
    public required string AccountId { get; init; }
    public required string Username { get; init; }
    public required string AvatarUrl { get; init; }
}