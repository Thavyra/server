using System.Collections.Immutable;

namespace Thavyra.Oidc.Models;

public class AuthorizationModel
{
    public Guid? ApplicationId { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public required Guid Id { get; set; }
    public ImmutableArray<string> Scopes { get; set; }
    public string? Status { get; set; }
    public Guid? Subject { get; set; }
    public string? Type { get; set; }
    
}