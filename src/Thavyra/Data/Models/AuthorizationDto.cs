using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("authorizations")]
public class AuthorizationDto
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Column("application_id")]
    public Guid? ApplicationId { get; set; }
    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("type")]
    public string? Type { get; set; }
    [Column("status")]
    public string? Status { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    public ApplicationDto Application { get; set; } = default!;
    public UserDto User { get; set; } = default!;
    public ICollection<TokenDto> Tokens { get; set; } = [];
    public ICollection<ScopeDto> Scopes { get; set; } = [];
}