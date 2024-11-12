using FastEndpoints;
using Thavyra.Rest.Services;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Users.GetAvatar;

public class Endpoint : Endpoint<Request>
{
    private readonly IIconService _iconService;
    private readonly IAvatarStorageService _storageService;

    public Endpoint(IIconService iconService, IAvatarStorageService storageService)
    {
        _iconService = iconService;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("/users/{User}/avatar.png");
        AllowAnonymous();
        Summary(x =>
        {
            x.Summary = "Get User Avatar";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().User is not { } user)
        {
            throw new InvalidOperationException();
        }

        var result = await _storageService.GetAvatarAsync(AvatarType.User, user.Id, ct);

        var image = result switch
        {
            GetFileSucceededResult file => file.Stream,

            _ => await _iconService.GetDefaultIconAsync(
                style: "glass",
                seed: user.Id.ToString(),
                size: req.Size ?? 500,
                cancellationToken: ct)
        };

        await SendStreamAsync(image, fileName: "avatar.png", contentType: "image/png", cancellation: ct);
    }
}