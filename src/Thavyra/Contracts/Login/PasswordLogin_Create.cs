namespace Thavyra.Contracts.Login;

public record PasswordLogin_Create
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}