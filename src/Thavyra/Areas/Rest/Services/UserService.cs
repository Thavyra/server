using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Services;

public class UserService : IUserService
{
    private readonly IRequestClient<User_ExistsByUsername> _usernameClient;
    private readonly IRequestClient<User_GetById> _getById;
    private readonly IRequestClient<User_GetByUsername> _getByUsername;
    private readonly IAuthorizationService _authorizationService;

    public UserService(
        IRequestClient<User_ExistsByUsername> usernameClient, 
        IRequestClient<User_GetById> getById, 
        IRequestClient<User_GetByUsername> getByUsername,
        IAuthorizationService authorizationService)
    {
        _usernameClient = usernameClient;
        _getById = getById;
        _getByUsername = getByUsername;
        _authorizationService = authorizationService;
    }
    
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken)
    {
        Response response = await _usernameClient.GetResponse<UsernameExists, NotFound>(new User_ExistsByUsername
        {
            Username = username
        }, cancellationToken);

        return response switch
        {
            (_, UsernameExists) => false,
            (_, NotFound) => true,
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<SlugResult<User>> GetUserFromRequestAsync(RequestWithAuthentication request, CancellationToken cancellationToken)
    {
        object? slug = request.UserSlug switch
        {
            "@me" when Guid.TryParse(request.Subject, out var subject) => subject,
            "@me" => new SlugClaimMissing<User>(),
            
            not null when Guid.TryParse(request.UserSlug, out var id) => id,
            not null when request.UserSlug.StartsWith('@') => request.UserSlug[1..],
            
            _ => new SlugInvalid<User>()
        };

        if (slug is SlugResult<User> result)
        {
            return result;
        }

        Response response = slug switch
        {
            Guid guid => await _getById.GetResponse<User, NotFound>(new User_GetById
            {
                Id = guid
            }, cancellationToken),
            
            string username => await _getByUsername.GetResponse<User, NotFound>(new User_GetByUsername
            {
                Username = username
            }, cancellationToken),
            
            _ => throw new InvalidOperationException()
        };

        return response switch
        {
            (_, User user) => new SlugFoundResult<User>(user),
            (_, NotFound) => new SlugNotFound<User>(),
            _ => throw new InvalidOperationException()
        };
    }

    public async Task<UserResponse> GetResponseAsync(User user, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var response = new UserResponse
        {
            Id = user.Id.ToString(),
            Username = user.Username
        };
        
        var profileAuthorizationResult = await _authorizationService.AuthorizeAsync(httpContext.User, user, Policies.Operation.User.Read);

        if (profileAuthorizationResult.Succeeded)
        {
            response.Description = JsonOptional<string?>.Of(user.Description);
        }

        return response;
    }
}