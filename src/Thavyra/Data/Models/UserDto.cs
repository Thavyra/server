using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("users")]
public class UserDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("username")]
    [MaxLength(40)]
    public string? Username { get; set; }
    [Column("description")]
    [MaxLength(400)]
    public string? Description { get; set; }

    [Column("balance")]
    public double Balance { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RoleDto> Roles { get; set; } = [];
}