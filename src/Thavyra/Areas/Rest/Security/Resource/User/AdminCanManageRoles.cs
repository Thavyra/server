using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;

namespace Thavyra.Rest.Security.Resource.User;

public class AdminCanManageRoles : AuthorizationHandler<ManageUserRolesRequirement, Contracts.User.User>
{
    private readonly IRequestClient<User_HasRole> _hasRole;

    public AdminCanManageRoles(IRequestClient<User_HasRole> hasRole)
    {
        _hasRole = hasRole;
    }
    
    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.User.User resource)
    {
        var response = await _hasRole.GetResponse<Correct, Incorrect, NotFound>(new User_HasRole
        {
            UserId = state.Subject,
            RoleName = Constants.Roles.Admin
        });

        if (!response.Is(out Response<Success> _))
        {
            return state;
        }

        return state
            .Succeed()
            .RequireScope(Constants.Scopes.Admin);
    }
}