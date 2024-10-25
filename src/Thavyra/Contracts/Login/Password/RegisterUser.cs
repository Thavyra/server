namespace Thavyra.Contracts.Login.Password;

public record RegisterUser
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}