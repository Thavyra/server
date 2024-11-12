namespace Thavyra.Contracts.Login.Data;

public record UserLoginResult
{
    public required IReadOnlyList<LoginResult> Logins { get; init; }
}