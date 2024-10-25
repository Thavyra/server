namespace Thavyra.Contracts.Login.Password;

public record PasswordLogin
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}