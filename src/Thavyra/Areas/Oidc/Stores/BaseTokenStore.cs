using System.Collections.Immutable;
using System.Text.Json;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Stores;

public abstract class BaseTokenStore : IOpenIddictTokenStore<TokenModel>
{
    // Repository methods
    
    public abstract ValueTask<TokenModel> InstantiateAsync(CancellationToken cancellationToken);

    public abstract ValueTask CreateAsync(TokenModel token, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(TokenModel token, CancellationToken cancellationToken);

    public abstract ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(TokenModel token, CancellationToken cancellationToken);
    
    public abstract ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken);

    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> ListAsync(int? count, int? offset, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindAsync(string subject, string client, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindAsync(string subject, string client, string status, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindAsync(string subject, string client, string status, string type,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<TokenModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<TokenModel?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<TokenModel> FindBySubjectAsync(string subject, CancellationToken cancellationToken);

    // EF

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<TokenModel>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<TokenModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();



    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<TokenModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    // Supported getters/setters

    public ValueTask<string?> GetApplicationIdAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.ApplicationId.ToString());

    public ValueTask SetApplicationIdAsync(TokenModel token, string? identifier, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var guid))
        {
            throw new ArgumentException("Not a valid guid.", nameof(identifier));
        }
        
        token.ApplicationId = guid;
        return new ValueTask();
    }


    public ValueTask<string?> GetAuthorizationIdAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.AuthorizationId.ToString());
    
    public ValueTask SetAuthorizationIdAsync(TokenModel token, string? identifier, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var guid))
        {
            throw new ArgumentException("Not a valid guid.", nameof(identifier));
        }
        
        token.AuthorizationId = guid;
        return new ValueTask();
    }


    public ValueTask<DateTimeOffset?> GetCreationDateAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.CreationDate);
    
    public ValueTask SetCreationDateAsync(TokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.CreationDate = date;
        return new ValueTask();
    }


    public ValueTask<DateTimeOffset?> GetExpirationDateAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.ExpirationDate);
    
    public ValueTask SetExpirationDateAsync(TokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.ExpirationDate = date;
        return new ValueTask();
    }


    public ValueTask<string?> GetIdAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.Id.ToString());


    public ValueTask<string?> GetPayloadAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.Payload);
    
    public ValueTask SetPayloadAsync(TokenModel token, string? payload, CancellationToken cancellationToken)
    {
        token.Payload = payload;
        return new ValueTask();
    }


    public ValueTask<DateTimeOffset?> GetRedemptionDateAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.RedemptionDate);
    
    public ValueTask SetRedemptionDateAsync(TokenModel token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.RedemptionDate = date;
        return new ValueTask();
    }


    public ValueTask<string?> GetReferenceIdAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.ReferenceId);
    
    public ValueTask SetReferenceIdAsync(TokenModel token, string? identifier, CancellationToken cancellationToken)
    {
        token.ReferenceId = identifier;
        return new ValueTask();
    }


    public ValueTask<string?> GetStatusAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.Status);
    
    public ValueTask SetStatusAsync(TokenModel token, string? status, CancellationToken cancellationToken)
    {
        token.Status = status;
        return new ValueTask();
    }


    public ValueTask<string?> GetSubjectAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.Subject.ToString());
    
    public ValueTask SetSubjectAsync(TokenModel token, string? subject, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(subject, out var guid))
        {
            throw new ArgumentException("Not a valid guid.", nameof(subject));
        }
        
        token.Subject = guid;
        return new ValueTask();
    }


    public ValueTask<string?> GetTypeAsync(TokenModel token, CancellationToken cancellationToken)
        => new(token.Type);
    
    public ValueTask SetTypeAsync(TokenModel token, string? type, CancellationToken cancellationToken)
    {
        token.Type = type;
        return new ValueTask();
    }
    
    // Unsupported getters/setters

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TokenModel token,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary<string, JsonElement>.Empty);

    public ValueTask SetPropertiesAsync(TokenModel token, ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => new();
}
