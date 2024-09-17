namespace Thavyra.Contracts.Login;

public record DiscordLogin_Create
{
    public required Guid UserId { get; init; }
    public required string DiscordId { get; init; }
}