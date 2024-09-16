using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("discord_logins")]
public class DiscordLoginDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("discord_id")] 
    public string DiscordId { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}