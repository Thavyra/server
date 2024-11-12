using System.Runtime.CompilerServices;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Token;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Stores;

public class TokenStore : BaseTokenStore
{
    private readonly IClientFactory _clientFactory;
    private readonly IPublishEndpoint _publishEndpoint;

    public TokenStore(IClientFactory clientFactory, IPublishEndpoint publishEndpoint)
    {
        _clientFactory = clientFactory;
        _publishEndpoint = publishEndpoint;
    }

    public override ValueTask<TokenModel> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<TokenModel>(new TokenModel
        {
            Id = NewId.NextGuid()
        });
    }

    private static TokenModel Map(Token token)
    {
        return new TokenModel
        {
            Id = token.Id,
            ApplicationId = token.ApplicationId,
            AuthorizationId = token.AuthorizationId,
            Subject = token.Subject,

            ReferenceId = token.ReferenceId,
            Type = token.Type,
            Status = token.Status,
            Payload = token.Payload,

            CreationDate = token.CreatedAt,
            RedemptionDate = token.RedeemedAt,
            ExpirationDate = token.ExpiresAt
        };
    }

    public override async IAsyncEnumerable<TokenModel> ListAsync(int? count, int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Token_List>();

        var response = await client.GetResponse<Multiple<Token>>(new Token_List
        {
            Count = count,
            Offset = offset
        }, cancellationToken);

        foreach (var token in response.Message.Items)
        {
            yield return Map(token);
        }
    }

    private async IAsyncEnumerable<TokenModel> GetAsync(string subject, string client, string? status = null,
        string? type = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(subject, out var userId))
        {
            yield break;
        }

        if (!Guid.TryParse(client, out var applicationId))
        {
            yield break;
        }
        
        var requestClient = _clientFactory.CreateRequestClient<Token_Get>();

        var response = await requestClient.GetResponse<Multiple<Token>>(new Token_Get
        {
            ApplicationId = applicationId,
            Subject = userId,
            Status = status,
            Type = type
        }, cancellationToken);

        foreach (var token in response.Message.Items)
        {
            yield return Map(token);
        }
    }

    public override IAsyncEnumerable<TokenModel> FindAsync(string subject, string client,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, cancellationToken: cancellationToken);
    }

    public override IAsyncEnumerable<TokenModel> FindAsync(string subject, string client, string status,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, status, cancellationToken: cancellationToken);
    }

    public override IAsyncEnumerable<TokenModel> FindAsync(string subject, string client, string status, string type,
        CancellationToken cancellationToken)
    {
        return GetAsync(subject, client, status, type, cancellationToken);
    }

    public override async IAsyncEnumerable<TokenModel> FindByApplicationIdAsync(string identifier,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var applicationId))
        {
            yield break;
        }
        
        var client = _clientFactory.CreateRequestClient<Token_GetByApplication>();

        var response = await client.GetResponse<Multiple<Token>>(new Token_GetByApplication
        {
            ApplicationId = applicationId
        }, cancellationToken);

        foreach (var token in response.Message.Items)
        {
            yield return Map(token);
        }
    }

    public override async IAsyncEnumerable<TokenModel> FindByAuthorizationIdAsync(string identifier,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var authorizationId))
        {
            yield break;
        }
        
        var client = _clientFactory.CreateRequestClient<Token_GetByAuthorization>();

        var response = await client.GetResponse<Multiple<Token>>(new Token_GetByAuthorization
        {
            AuthorizationId = authorizationId
        }, cancellationToken);

        foreach (var token in response.Message.Items)
        {
            yield return Map(token);
        }
    }

    public override async ValueTask<TokenModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var id))
        {
            return null;
        }
        
        var client = _clientFactory.CreateRequestClient<Token_GetById>();

        Response response = await client.GetResponse<Token, NotFound>(new Token_GetById
        {
            Id = id
        }, cancellationToken);

        return response switch
        {
            (_, Token token) => Map(token),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async ValueTask<TokenModel?> FindByReferenceIdAsync(string identifier,
        CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Token_GetByReferenceId>();

        Response response = await client.GetResponse<Token, NotFound>(new Token_GetByReferenceId
        {
            ReferenceId = identifier
        }, cancellationToken);

        return response switch
        {
            (_, Token token) => Map(token),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async IAsyncEnumerable<TokenModel> FindBySubjectAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(subject, out var userId))
        {
            yield break;
        }
        
        var client = _clientFactory.CreateRequestClient<Token_GetBySubject>();

        var response = await client.GetResponse<Multiple<Token>>(new Token_GetBySubject
        {
            Subject = userId
        }, cancellationToken);

        foreach (var token in response.Message.Items)
        {
            yield return Map(token);
        }
    }

    public override async ValueTask CreateAsync(TokenModel token, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Token_Create>();

        await client.GetResponse<Token>(new Token_Create
        {
            Id = token.Id,
            ApplicationId = token.ApplicationId,
            AuthorizationId = token.AuthorizationId,
            Subject = token.Subject,

            ReferenceId = token.ReferenceId,
            Type = token.Type,
            Status = token.Status,
            Payload = token.Payload,

            CreatedAt = token.CreationDate?.UtcDateTime ?? DateTime.UtcNow,
            RedeemedAt = token.RedemptionDate?.UtcDateTime,
            ExpiresAt = token.ExpirationDate?.UtcDateTime
        }, cancellationToken);
    }

    public override async ValueTask DeleteAsync(TokenModel token, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Token_Delete
        {
            Id = token.Id
        }, cancellationToken);
    }

    public override async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Token_Prune>();

        var response = await client.GetResponse<Count>(new Token_Prune
        {
            Threshold = threshold.UtcDateTime
        }, cancellationToken);

        return response.Message.Value;
    }

    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Token_Count>();

        var response = await client.GetResponse<Count>(new Token_Count(), cancellationToken);

        return response.Message.Value;
    }

    public override async ValueTask UpdateAsync(TokenModel token, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Token_Update
        {
            Id = token.Id,
            
            ReferenceId = token.ReferenceId,
            Type = token.Type,
            Status = token.Status,
            Payload = token.Payload,

            RedeemedAt = token.RedemptionDate?.UtcDateTime,
            ExpiresAt = token.ExpirationDate?.UtcDateTime
        }, cancellationToken);
    }

    public override async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var authorizationId))
        {
            return 0;
        }
        
        var client = _clientFactory.CreateRequestClient<Token_RevokeByAuthorization>();

        var response = await client.GetResponse<Count>(new Token_RevokeByAuthorization
        {
            AuthorizationId = authorizationId
        }, cancellationToken);

        return response.Message.Value;
    }
}