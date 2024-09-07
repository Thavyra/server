namespace Thavyra.Contracts.Login;

/// <summary>
/// Checks that the specified user has a password login, and that the specified password is correct.
/// </summary>
/// <returns><see cref="PasswordCorrect"/>, <see cref="PasswordIncorrect"/></returns>
public record PasswordLogin_Check
{
    public required Guid UserId { get; init; }
    public required string Password { get; init; }
}