using Thavyra.Contracts.User;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Services;

public interface IUserService
{
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken);
    Task<SlugResult<User>> GetUserFromRequestAsync(RequestWithAuthentication request, CancellationToken cancellationToken);
    Task<UserResponse> GetResponseAsync(User user, HttpContext httpContext, CancellationToken cancellationToken);
}