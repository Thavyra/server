using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("github_logins")]
public class GitHubLoginDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("github_id")] 
    public string GitHubId { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}