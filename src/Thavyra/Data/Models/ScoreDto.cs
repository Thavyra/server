using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("scores")]
public class ScoreDto
{
    [Column("id")] 
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("objective_id")]
    public Guid ObjectiveId { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("value")]
    public double Value { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ObjectiveDto? Objective { get; set; }
    public UserDto? User { get; set; }
}