using Thavyra.Contracts.Login;

namespace Thavyra.Contracts.User.Register;

public record DiscordRegistration
{
    public required User User { get; init; }
    public required DiscordLogin Login { get; init; }
}