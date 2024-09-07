using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("passwords")]
public class PasswordLoginDto
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("hashed_password")]
    [MaxLength(100)]
    public string HashedPassword { get; set; } = default!;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public UserDto User { get; set; } = default!;
}