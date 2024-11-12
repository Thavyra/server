using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;
using Thavyra.Contracts.User;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public class UserManager : IUserManager
{
    private readonly IRequestClient<User_GetById> _getUserById;
    private readonly IRequestClient<User_ExistsByUsername> _existsByUsername;
    private readonly IRequestClient<Role_GetByUser> _getRoles;

    public UserManager(
        IRequestClient<User_GetById> getUserById,
        IRequestClient<User_ExistsByUsername> existsByUsername,
        IRequestClient<Role_GetByUser> getRoles)
    {
        _getUserById = getUserById;
        _existsByUsername = existsByUsername;
        _getRoles = getRoles;
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken)
    {
        Response response = await _existsByUsername.GetResponse<UsernameExists, NotFound>(new User_ExistsByUsername
        {
            Username = username
        }, cancellationToken);

        return response switch
        {
            (_, UsernameExists) => false, // Username is not unique
            (_, NotFound) => true, // Username is unique
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<IReadOnlyList<RoleModel>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _getRoles.GetResponse<Multiple<Role>>(new Role_GetByUser
        {
            UserId = userId
        }, cancellationToken);

        return response.Message.Items.Select(x => new RoleModel
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }
}