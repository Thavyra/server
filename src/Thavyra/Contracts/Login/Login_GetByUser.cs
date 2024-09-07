namespace Thavyra.Contracts.Login;

/// <summary>
/// Retrieves the logins associated with the specified user. Login types can be filtered using request accept types. <see cref="NotFound"/> will be returned if the user does not have an accepted login type.
/// </summary>
/// <returns><see cref="PasswordLogin"/>, <see cref="NotFound"/></returns>
public record Login_GetByUser
{
    public required Guid UserId { get; init; }
}