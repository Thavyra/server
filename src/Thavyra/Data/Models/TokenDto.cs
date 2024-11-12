using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("tokens")]
public class TokenDto
{
    [Column("id")] 
    public Guid Id { get; set; } = NewId.NextGuid();

    [Column("application_id")]
    public Guid? ApplicationId { get; set; }
    
    [Column("authorization_id")]
    public Guid? AuthorizationId { get; set; }
    
    [Column("subject")]
    public Guid? Subject { get; set; }
    
    [Column("reference_id"), MaxLength(256)]
    public string? ReferenceId { get; set; }
    
    [Column("type"), MaxLength(40)]
    public string? Type { get; set; }
    
    [Column("status"), MaxLength(40)]
    public string? Status { get; set; }
    
    [Column("payload"), MaxLength(4096)]
    public string? Payload { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("redeemed_at")]
    public DateTime? RedeemedAt { get; set; }
    
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    public ApplicationDto? Application { get; set; }
    public AuthorizationDto? Authorization { get; set; }
}