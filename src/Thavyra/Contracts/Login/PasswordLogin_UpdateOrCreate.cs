namespace Thavyra.Contracts.Login;

public record PasswordLogin_UpdateOrCreate
{
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}