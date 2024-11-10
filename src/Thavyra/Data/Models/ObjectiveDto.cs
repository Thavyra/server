using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("objectives")]
public class ObjectiveDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    [Column("name")]
    public string Name { get; set; } = default!;

    [Column("display_name")]
    public string DisplayName { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }

    public ApplicationDto Application { get; set; } = default!;
    public ICollection<ScoreDto> Scores { get; set; } = default!;
}