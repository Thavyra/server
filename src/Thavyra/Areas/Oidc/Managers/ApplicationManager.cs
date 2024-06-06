using MassTransit;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Oidc.Models;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public class ApplicationManager : OpenIddictApplicationManager<ApplicationModel>
{
    private readonly IRequestClient<Application_ValidateClientSecret> _client;

    public ApplicationManager(
        IRequestClient<Application_ValidateClientSecret> client,
        IOpenIddictApplicationCache<ApplicationModel> cache,
        ILogger<ApplicationManager> logger,
        IOptionsMonitor<OpenIddictCoreOptions> options,
        IOpenIddictApplicationStoreResolver resolver)
        : base(cache, logger, options, resolver)
    {
        _client = client;
    }

    public override async ValueTask<bool> ValidateClientSecretAsync(ApplicationModel application, string secret,
        CancellationToken cancellationToken = new())
    {
        var response = await _client.GetResponse<Value<bool>>(new Application_ValidateClientSecret
        {
            ApplicationId = application.Id,
            Secret = secret
        }, cancellationToken);

        return response.Message.Item;
    }
}