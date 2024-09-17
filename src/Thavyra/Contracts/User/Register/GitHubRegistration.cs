using Thavyra.Contracts.Login;

namespace Thavyra.Contracts.User.Register;

public record GitHubRegistration
{
    public required User User { get; init; }
    public required GitHubLogin Login { get; init; }
}