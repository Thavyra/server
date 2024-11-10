namespace Thavyra.Contracts.User;

public record UserCreated
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required DateTime Timestamp { get; init; }
}