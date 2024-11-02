using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("logins")]
public class LoginDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("type")]
    public string Type { get; set; } = null!;

    [Column("password_hash")]
    public string? PasswordHash { get; set; }
    
    [Column("provider_account_id")]
    public string? ProviderAccountId { get; set; }
    [Column("provider_username")]
    public string? ProviderUsername { get; set; }
    [Column("provider_avatar_url")]
    public string? ProviderAvatarUrl { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserDto User { get; set; } = null!;
    public ICollection<LoginAttemptDto> Attempts { get; set; } = [];
}