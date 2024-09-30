namespace Thavyra.Contracts.Role;

public record User_GrantRole
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}