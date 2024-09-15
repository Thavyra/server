using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("scores")]
public class ScoreDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("objective_id")]
    public Guid ObjectiveId { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("value")]
    public double Value { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ObjectiveDto Objective { get; set; } = default!;
    public UserDto User { get; set; } = default!;
}