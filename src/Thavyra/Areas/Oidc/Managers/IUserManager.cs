using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public interface IUserManager
{
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken);
    Task<IReadOnlyList<RoleModel>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);
}