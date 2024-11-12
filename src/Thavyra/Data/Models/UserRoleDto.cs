using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("user_roles")]
public class UserRoleDto
{
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("role_id")]
    public Guid RoleId { get; set; }
}