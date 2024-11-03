using FastEndpoints;
using OpenIddict.Abstractions;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.GetAvatar;

public class Endpoint : Endpoint<Request>
{
    private readonly IIconService _iconService;

    public Endpoint(IIconService iconService)
    {
        _iconService = iconService;
    }

    public override void Configure()
    {
        Get("/users/{User}/avatar.png");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().User is not { } user)
        {
            throw new InvalidOperationException();
        }
        
        var image = await _iconService.GetDefaultIconAsync(
            style: "avataaars-neutral",
            seed: user.Username,
            size: req.Size ?? 128,
            cancellationToken: ct);

        await SendStreamAsync(image, fileName: "avatar.png", contentType: "image/png", cancellation: ct);
    }
}