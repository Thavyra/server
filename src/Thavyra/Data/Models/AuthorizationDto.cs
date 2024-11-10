using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("authorizations")]
public class AuthorizationDto
{
    [Column("id")] 
    public Guid Id { get; set; } = NewId.NextGuid();

    [Column("application_id")] 
    public Guid? ApplicationId { get; set; }

    [Column("subject")] 
    public Guid? Subject { get; set; }

    [Column("type"), MaxLength(40)] 
    public string? Type { get; set; }

    [Column("status"), MaxLength(40)] 
    public string? Status { get; set; }

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public ApplicationDto? Application { get; set; }
    public ICollection<TokenDto> Tokens { get; set; } = [];
    public ICollection<ScopeDto> Scopes { get; set; } = [];
}