using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("roles")]
public class RoleDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("display_name")]
    public string DisplayName { get; set; } = null!;

    public ICollection<UserDto> Users { get; set; } = [];
}