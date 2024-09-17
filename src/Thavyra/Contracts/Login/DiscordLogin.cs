namespace Thavyra.Contracts.Login;

public record DiscordLogin
{
    public required Guid Id { get; init; }
    public required string DiscordId { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime CreatedAt { get; init; }
}