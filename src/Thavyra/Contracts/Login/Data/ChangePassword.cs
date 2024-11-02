namespace Thavyra.Contracts.Login.Data;

public record ChangePassword
{
    public required Guid UserId { get; init; }
    public required string CurrentPassword { get; init; }
    public required string Password { get; init; }
}