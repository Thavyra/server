using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("permissions")]
public class PermissionDto
{
    [Column("id")]
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("name"), MaxLength(40)]
    public string Name { get; set; } = null!;
    
    [Column("display_name"), MaxLength(40)]
    public string DisplayName { get; set; } = null!;

    [Column("scope_id")]
    public Guid? ScopeId { get; set; }

    public ScopeDto? Scope { get; set; }
    public ICollection<ApplicationDto> Applications { get; set; } = [];
}