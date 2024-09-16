namespace Thavyra.Contracts.Login;

public record GitHubLogin_GetOrCreate
{
    public required string GitHubId { get; init; }
    public required string Username { get; init; }
}