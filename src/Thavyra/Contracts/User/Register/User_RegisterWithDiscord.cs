namespace Thavyra.Contracts.User.Register;

public record User_RegisterWithDiscord
{
    public required string DiscordId { get; init; }
    public required string Username { get; init; }
}