using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("roles")]
public class RoleDto
{
    [Column("id")]
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("name"), MaxLength(40)]
    public string Name { get; set; } = null!;
    
    [Column("display_name"), MaxLength(40)]
    public string DisplayName { get; set; } = null!;

    public ICollection<UserDto> Users { get; set; } = [];
}