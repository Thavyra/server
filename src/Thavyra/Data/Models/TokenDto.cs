using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("tokens")]
public class TokenDto
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("application_id")]
    public Guid? ApplicationId { get; set; }
    [Column("authorization_id")]
    public Guid? AuthorizationId { get; set; }
    [Column("subject")]
    public Guid? Subject { get; set; }
    
    [Column("reference_id")]
    public string? ReferenceId { get; set; }
    [Column("type")]
    public string? Type { get; set; }
    [Column("status")]
    public string? Status { get; set; }
    [Column("payload")]
    public string? Payload { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("redeemed_at")]
    public DateTime? RedeemedAt { get; set; }
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    public ApplicationDto? Application { get; set; }
    public AuthorizationDto? Authorization { get; set; }
}