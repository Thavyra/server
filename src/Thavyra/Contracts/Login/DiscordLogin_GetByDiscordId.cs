namespace Thavyra.Contracts.Login;

public record DiscordLogin_GetByDiscordId
{
    public required string DiscordId { get; init; }
}