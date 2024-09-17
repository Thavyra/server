namespace Thavyra.Contracts.Login;

public record GitHubLogin_GetByGitHubId
{
    public required string GitHubId { get; init; }
}