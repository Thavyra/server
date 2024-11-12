namespace Thavyra.Contracts.Login.Data;

public record PasswordChanged
{
    public required Guid LoginId { get; init; }
    public required DateTime Timestamp { get; init; }
}