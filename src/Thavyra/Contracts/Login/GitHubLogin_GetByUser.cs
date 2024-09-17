namespace Thavyra.Contracts.Login;

public record GitHubLogin_GetByUser
{
    public required Guid UserId { get; init; }   
}