namespace Thavyra.Contracts.Login.Data;

public record GetUserLogins
{
    public required Guid UserId { get; init; }
}