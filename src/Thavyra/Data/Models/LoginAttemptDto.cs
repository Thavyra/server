using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("login_attempts")]
public class LoginAttemptDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("login_id")]
    public Guid LoginId { get; set; }
    [Column("succeeded")]
    public bool Succeeded { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public LoginDto Login { get; set; } = null!;
}