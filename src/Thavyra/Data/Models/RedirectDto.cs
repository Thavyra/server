using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("redirects")]
public class RedirectDto
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("application_id")]
    public Guid ApplicationId { get; set; }

    [Column("uri")]
    public string Uri { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ApplicationDto Application { get; set; } = default!;
}