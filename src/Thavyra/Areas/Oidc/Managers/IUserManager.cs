using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public interface IUserManager
{
    Task<UserModel> RegisterAsync(PasswordRegisterModel login, CancellationToken cancellationToken);
    Task<UserModel?> FindByIdAsync(string id, CancellationToken cancellationToken);
    Task<UserModel?> FindByLoginAsync(PasswordLoginModel login, CancellationToken cancellationToken);
    Task<UserModel> FindOrCreateByLoginAsync(DiscordLoginModel login, CancellationToken cancellationToken);
    Task<UserModel> FindOrCreateByLoginAsync(GitHubLoginModel login, CancellationToken cancellationToken);
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken);
}