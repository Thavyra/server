namespace Thavyra.Contracts.Login;

public record LoginFailed
{
    public required Guid LoginId { get; init; }
    public required DateTime Timestamp { get; init; }
}