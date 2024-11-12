using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("authorization_scopes")]
public class AuthorizationScopeDto
{
    [Column("authorization_id")]
    public Guid AuthorizationId { get; set; }
    
    [Column("scope_id")]
    public Guid ScopeId { get; set; }
}