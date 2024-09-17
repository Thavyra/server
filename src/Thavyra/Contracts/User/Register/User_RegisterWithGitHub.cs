namespace Thavyra.Contracts.User.Register;

public record User_RegisterWithGitHub
{
    public required string GitHubId { get; init; }
    public required string Username { get; init; }
}