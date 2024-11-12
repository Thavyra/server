using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Users.DeleteAvatar;

public class Endpoint : Endpoint<UserRequest>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IAvatarStorageService _storage;

    public Endpoint(IAuthorizationService authorizationService, IAvatarStorageService storage)
    {
        _authorizationService = authorizationService;
        _storage = storage;
    }

    public override void Configure()
    {
        Delete("/users/{User}/avatar");
        Summary(x =>
        {
            x.Summary = "Delete Avatar";
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().User is not { } user)
        {
            throw new InvalidOperationException();
        }

        if (await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.UpdateProfile) is
            { Succeeded: false, Failure: var failure })
        {
            await this.SendAuthorizationFailureAsync(failure, ct);
            return;
        }

        var result = await _storage.DeleteAvatarAsync(AvatarType.User, user.Id, ct);

        if (result is DeleteFileSucceededResult or DeleteFileNotFoundResult)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendErrorsAsync(cancellation: ct);
    }
}