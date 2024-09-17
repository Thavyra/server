namespace Thavyra.Contracts.Login;

public record PasswordLogin_GetByUser
{
    public required Guid UserId { get; init; }   
}