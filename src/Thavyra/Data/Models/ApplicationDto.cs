using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("applications")]
public class ApplicationDto
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("owner_id")]
    public Guid OwnerId { get; set; }
    
    [Column("client_id")]
    public string ClientId { get; set; } = default!;
    
    [Column("client_secret_hash")]
    public string? ClientSecretHash { get; set; }

    [Column("type")]
    public string Type { get; set; } = default!;
    [Column("name")]
    public string Name { get; set; } = default!;
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }

    public UserDto Owner { get; set; } = default!;
    public ICollection<RedirectDto> Redirects { get; set; } = default!;
    public ICollection<PermissionDto> Permissions { get; set; } = default!;
    public ICollection<ObjectiveDto> Objectives { get; set; } = default!;
}