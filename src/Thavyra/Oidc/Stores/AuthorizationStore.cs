using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Oidc.Models;

namespace Thavyra.Oidc.Stores;

public class AuthorizationStore : BaseAuthorizationStore
{
    private readonly IScopedClientFactory _clientFactory;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthorizationStore(IScopedClientFactory clientFactory, IPublishEndpoint publishEndpoint)
    {
        _clientFactory = clientFactory;
        _publishEndpoint = publishEndpoint;
    }

    public override ValueTask<AuthorizationModel> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<AuthorizationModel>(new AuthorizationModel
        {
            Id = NewId.NextGuid()
        });
    }

    private static AuthorizationModel Map(Authorization authorization)
    {
        return new AuthorizationModel
        {
            Id = authorization.Id,
            ApplicationId = authorization.ApplicationId,
            Subject = authorization.UserId,

            Type = authorization.Type,
            Status = authorization.Status,

            Scopes = authorization.Scopes.ToImmutableArray(),

            CreationDate = authorization.CreatedAt
        };
    }

    public override async IAsyncEnumerable<AuthorizationModel> ListAsync(int? count, int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_List>();

        var response = await client.GetResponse<Multiple<Authorization>>(new Authorization_List
        {
            Count = count,
            Offset = offset
        }, cancellationToken);

        foreach (var authorization in response.Message.Items)
        {
            yield return Map(authorization);
        }
    }

    private async IAsyncEnumerable<AuthorizationModel> GetAsync(string subject, string client, string? status = null,
        string? type = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestClient = _clientFactory.CreateRequestClient<Authorization_Get>();

        var response = await requestClient.GetResponse<Multiple<Authorization>>(new Authorization_Get
        {
            ApplicationId = client,
            UserId = subject,
            Status = status,
            Type = type
        }, cancellationToken);

        foreach (var authorization in response.Message.Items)
        {
            yield return Map(authorization);
        }
    }

    public override IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, cancellationToken: cancellationToken);
    }

    public override IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, status, cancellationToken: cancellationToken);
    }

    public override IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status,
        string type,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, status, type, cancellationToken);
    }

    public override async IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status,
        string type, ImmutableArray<string> scopes,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var authorizations = GetAsync(subject, client, status, type, cancellationToken);

        await foreach (var authorization in authorizations)
        {
            if (authorization.Scopes.Order().SequenceEqual(scopes.Order()))
            {
                yield return authorization;
            }
        }
    }

    public override async IAsyncEnumerable<AuthorizationModel> FindByApplicationIdAsync(string identifier,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_GetById>();

        var response = await client.GetResponse<Multiple<Authorization>>(new Authorization_GetByApplication
        {
            ApplicationId = identifier
        }, cancellationToken);

        foreach (var authorization in response.Message.Items)
        {
            yield return Map(authorization);
        }
    }

    public override async ValueTask<AuthorizationModel?> FindByIdAsync(string identifier,
        CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_GetById>();

        Response response = await client.GetResponse<Authorization, NotFound>(new Authorization_GetById
        {
            Id = identifier
        }, cancellationToken);

        return response switch
        {
            (_, Authorization authorization) => Map(authorization),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async IAsyncEnumerable<AuthorizationModel> FindBySubjectAsync(string subject,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_GetByUser>();

        var response = await client.GetResponse<Multiple<Authorization>>(new Authorization_GetByUser
        {
            UserId = subject
        }, cancellationToken);

        foreach (var authorization in response.Message.Items)
        {
            yield return Map(authorization);
        }
    }

    public override async ValueTask CreateAsync(AuthorizationModel authorization, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Authorization_Create
        {
            Id = authorization.Id,
            ApplicationId = authorization.ApplicationId ?? null,
            UserId = authorization.Subject ?? null,
            Type = authorization.Type,
            Status = authorization.Status,
            Scopes = authorization.Scopes.ToList(),
            CreatedAt = authorization.CreationDate?.UtcDateTime ?? DateTime.UtcNow
        }, cancellationToken);
    }

    public override async ValueTask DeleteAsync(AuthorizationModel authorization, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Authorization_Delete
        {
            Id = authorization.Id
        }, cancellationToken);
    }

    public override async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_Prune>();

        var response = await client.GetResponse<Value<long>>(new Authorization_Prune
        {
            Threshold = threshold.UtcDateTime
        }, cancellationToken);

        return response.Message.Item;
    }

    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Authorization_Count>();

        var response = await client.GetResponse<Value<long>>(new Authorization_Count(), cancellationToken);

        return response.Message.Item;
    }

    public override async ValueTask UpdateAsync(AuthorizationModel authorization, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Authorization_Update
        {
            Id = authorization.Id,

            Type = authorization.Type,
            Status = authorization.Status
        }, cancellationToken);
    }
}