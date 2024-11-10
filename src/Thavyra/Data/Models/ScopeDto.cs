using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("scopes")]
public class ScopeDto
{
    [Column("id")]
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("name"), MaxLength(40)]
    public string Name { get; set; } = default!;
    
    [Column("display_name"), MaxLength(40)]
    public string DisplayName { get; set; } = default!;
    
    [Column("description"), MaxLength(400)]
    public string Description { get; set; } = default!;


    public PermissionDto? Permission { get; set; }
    public ICollection<AuthorizationDto> Authorizations { get; set; } = [];
}