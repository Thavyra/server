namespace Thavyra.Contracts.Login;

public record GitHubLogin_Create
{
    public required Guid UserId { get; init; }
    public required string GitHubId { get; init; }
}