namespace Thavyra.Contracts.Login;

/// <summary>
/// Checks that the specified user has a password login, and that the specified password is correct. Returns <see cref="NotFound"/> if a password login does not exist.
/// </summary>
/// <returns><see cref="Correct"/>, <see cref="Incorrect"/>, <see cref="NotFound"/></returns>
public record PasswordLogin_Check
{
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}