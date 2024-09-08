using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("scopes")]
public class ScopeDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = default!;
    [Column("display_name")]
    public string DisplayName { get; set; } = default!;
    [Column("description")]
    public string Description { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ICollection<AuthorizationDto> Authorizations { get; set; } = [];
}