using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public interface ILoginManager
{
    Task<LoginModel> RegisterAsync(RegisterModel model, CancellationToken cancellationToken);
    Task<LoginModel?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
}