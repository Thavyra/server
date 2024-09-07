namespace Thavyra.Contracts.User;

/// <summary>
/// Checks whether the specified username is in use. 
/// </summary>
/// <returns><see cref="UsernameExists"/>, <see cref="NotFound"/></returns>
public record User_ExistsByUsername
{
    public required string Username { get; init; }
}