using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("system")]
public class SystemDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    public ApplicationDto Application { get; set; } = null!;
}