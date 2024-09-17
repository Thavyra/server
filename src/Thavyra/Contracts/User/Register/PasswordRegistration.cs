using Thavyra.Contracts.Login;

namespace Thavyra.Contracts.User.Register;

public record PasswordRegistration
{
    public required User User { get; init; }
    public required PasswordLogin Login { get; init; }
};