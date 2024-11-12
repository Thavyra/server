using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Users.PutAvatar;

public class Endpoint : Endpoint<Request>
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
        Put("/users/{User}/avatar");
        AllowFileUploads();
        Summary(x =>
        {
            x.Summary = "Upload User Avatar";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
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
        
        var result = await _storage.UploadAvatarAsync(AvatarType.User, user.Id, req.Avatar.OpenReadStream(), ct);

        if (result is not UploadFileSucceededResult)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        await SendCreatedAtAsync<GetAvatar.Endpoint>(new { User = user.Id }, null, cancellation: ct);
    }
}