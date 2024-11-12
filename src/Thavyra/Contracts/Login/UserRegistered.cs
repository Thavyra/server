namespace Thavyra.Contracts.Login;

public record UserRegistered
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}