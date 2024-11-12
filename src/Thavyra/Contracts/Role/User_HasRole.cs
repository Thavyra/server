namespace Thavyra.Contracts.Role;

public record User_HasRole
{
    public Guid UserId { get; set; }
    public required string RoleName { get; set; }   
}