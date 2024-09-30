namespace Thavyra.Contracts.Role;

public record Role_GetByUser
{
    public required Guid UserId { get; init; }
}