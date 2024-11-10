using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("applications")]
public class ApplicationDto
{
    [Column("id")] 
    public Guid Id { get; set; } = NewId.NextGuid();

    [Column("owner_id")]
    public Guid OwnerId { get; set; }
    
    [Column("client_id"), MaxLength(40)]
    public string ClientId { get; set; } = default!;
    
    [Column("client_secret_hash"), MaxLength(60)]
    public string? ClientSecretHash { get; set; }

    [Column("type"), MaxLength(40)]
    public string Type { get; set; } = default!;
    
    [Column("name"), MaxLength(40)]
    public string Name { get; set; } = default!;
    
    [Column("description"), MaxLength(400)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public UserDto? Owner { get; set; }
    
    public ICollection<RedirectDto> Redirects { get; set; } = [];
    public ICollection<PermissionDto> Permissions { get; set; } = [];
    public ICollection<ObjectiveDto> Objectives { get; set; } = [];
}