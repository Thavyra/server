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
    public string Username { get; set; } = default!;
    [Column("description")]
    [MaxLength(400)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}