using FastEndpoints;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.User;

namespace Thavyra.Rest.Features.Users;

public class UserSlugParser : GlobalPreProcessor<UserRequestState>
{
    private readonly IRequestClient<User_GetById> _idClient;
    private readonly IRequestClient<User_GetByUsername> _usernameClient;

    public UserSlugParser(IRequestClient<User_GetById> idClient, IRequestClient<User_GetByUsername> usernameClient)
    {
        _idClient = idClient;
        _usernameClient = usernameClient;
    }

    public override async Task PreProcessAsync(IPreProcessorContext context, UserRequestState requestState, CancellationToken ct)
    {
        if (context.Request is not UserRequest request)
        {
            return;
        }
        
        Response? userResponse = null;

        if (Guid.TryParse(request.User, out var id))
        {
            userResponse = await _idClient.GetResponse<User, NotFound>(new User_GetById
            {
                Id = id
            }, ct);
        }
        
        if (request.User == "@me")
        {
            if (request.Subject == default)
            {
                await context.HttpContext.Response.SendUnauthorizedAsync(ct);
                return;
            }
            
            userResponse = await _idClient.GetResponse<User, NotFound>(new User_GetById
            {
                Id = request.Subject
            }, ct);
        }
        
        if (request.User?.StartsWith('@') is true)
        {
            userResponse = await _usernameClient.GetResponse<User, NotFound>(new User_GetByUsername
            {
                Username = request.User[1..]
            }, ct);
        }
        
        switch (userResponse)
        {
            case (_, NotFound):
                await context.HttpContext.Response.SendNotFoundAsync(cancellation: ct);
                return;
            case (_, User user):
                requestState.User = user;
                break;
        }
    }
}