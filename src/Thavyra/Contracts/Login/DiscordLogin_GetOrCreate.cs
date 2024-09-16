namespace Thavyra.Contracts.Login;

public record DiscordLogin_GetOrCreate
{
    public required string DiscordId { get; init; }
    public required string Username { get; init; }
}