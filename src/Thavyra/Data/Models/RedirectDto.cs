using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("redirects")]
public class RedirectDto
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = NewId.NextGuid();

    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    [Column("uri"), MaxLength(2048)]
    public string Uri { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationDto? Application { get; set; }
}