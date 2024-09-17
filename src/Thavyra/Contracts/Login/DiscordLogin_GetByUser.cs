namespace Thavyra.Contracts.Login;

public record DiscordLogin_GetByUser
{
    public required Guid UserId { get; init; }
};