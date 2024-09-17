namespace Thavyra.Contracts.Login;

public record PasswordLogin_Update
{
    public required Guid Id { get; init; }
    public required string Password { get; init; }
}