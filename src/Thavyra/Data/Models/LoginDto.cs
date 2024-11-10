using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("logins")]
public class LoginDto
{
    [Column("id")]
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("type"), MaxLength(40)]
    public string Type { get; set; } = null!;

    [Column("password_hash"), MaxLength(60)]
    public string? PasswordHash { get; set; }
    
    [Column("provider_account_id"), MaxLength(128)]
    public string? ProviderAccountId { get; set; }
    
    [Column("provider_username"), MaxLength(100)]
    public string? ProviderUsername { get; set; }
    
    [Column("provider_avatar_url"), MaxLength(2048)]
    public string? ProviderAvatarUrl { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserDto? User { get; set; }
    
    public ICollection<LoginAttemptDto> Attempts { get; set; } = [];
}