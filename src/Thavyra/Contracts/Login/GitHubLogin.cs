namespace Thavyra.Contracts.Login;

public record GitHubLogin
{
    public required Guid Id { get; init; }
    public required string GitHubId { get; init; }
    public required User.User User { get; init; }
    public required DateTime CreatedAt { get; init; }
}