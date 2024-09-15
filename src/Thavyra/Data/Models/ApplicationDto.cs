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
    [Column("client_secret")]
    public string? ClientSecret { get; set; }
    [Column("client_type")]
    public string ClientType { get; set; } = default!;
    [Column("consent_type")]
    public string ConsentType { get; set; } = default!;

    [Column("type")]
    public string Type { get; set; } = default!;
    [Column("name")]
    public string Name { get; set; } = default!;
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public UserDto Owner { get; set; } = default!;
    public ICollection<RedirectDto> Redirects { get; set; } = default!;
    public ICollection<ObjectiveDto> Objectives { get; set; } = default!;
}