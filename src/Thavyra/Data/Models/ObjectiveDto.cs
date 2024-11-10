using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("objectives")]
public class ObjectiveDto
{
    [Column("id")] 
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    [Column("name"), MaxLength(40)]
    public string Name { get; set; } = default!;

    [Column("display_name"), MaxLength(40)]
    public string DisplayName { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }

    public ApplicationDto? Application { get; set; }
    public ICollection<ScoreDto> Scores { get; set; } = [];
}