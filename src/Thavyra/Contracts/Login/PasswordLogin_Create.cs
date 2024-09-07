namespace Thavyra.Contracts.Login;

public record PasswordLogin_Create
{
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}