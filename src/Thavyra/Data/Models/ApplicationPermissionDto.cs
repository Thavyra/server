using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("application_permissions")]
public class ApplicationPermissionDto
{
    [Column("application_id")]
    public Guid ApplicationId { get; set; }
    
    [Column("permission_id")]
    public Guid PermissionId { get; set; }
}