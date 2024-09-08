using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Models.View;

public class AuthorizeViewModel
{
    public required string ReturnUrl { get; set; }
    
    public required ApplicationModel Client { get; set; }
    public required UserModel Subject { get; set; }
    public required IReadOnlyList<ScopeModel> Scopes { get; set; }
}