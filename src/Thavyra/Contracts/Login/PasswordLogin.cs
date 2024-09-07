namespace Thavyra.Contracts.Login;

public record PasswordLogin
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}