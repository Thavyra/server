using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("permissions")]
public class PermissionDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("display_name")]
    public string DisplayName { get; set; } = null!;

    [Column("scope_id")]
    public Guid? ScopeId { get; set; }

    public ScopeDto? Scope { get; set; } = null!;
    public ICollection<ApplicationDto> Applications { get; set; } = default!;
}