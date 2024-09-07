namespace Thavyra.Contracts.User;

/// <summary>
/// Creates a new user.
/// </summary>
/// <returns><see cref="User"/></returns>
public record User_Create
{
    public required string Username { get; init; }
}