namespace Thavyra.Contracts.Login;

public record LoginSucceeded
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}