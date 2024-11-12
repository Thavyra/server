namespace Thavyra.Contracts.User;

/// <summary>
/// Creates a new user.
/// </summary>
public record CreateUser
{
    public string? Username { get; init; }
}