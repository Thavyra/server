using System.Collections.Immutable;
using System.Text.Json;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Models;

namespace Thavyra.Oidc.Stores;

public abstract class BaseAuthorizationStore : IOpenIddictAuthorizationStore<AuthorizationModel>
{
    // Repository methods
    
    public abstract ValueTask<AuthorizationModel> InstantiateAsync(CancellationToken cancellationToken);

    public abstract ValueTask CreateAsync(AuthorizationModel authorization, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(AuthorizationModel authorization, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(AuthorizationModel authorization, CancellationToken cancellationToken);

    public abstract ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken);

    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> ListAsync(int? count, int? offset, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status, string type,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<AuthorizationModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<AuthorizationModel> FindBySubjectAsync(string subject, CancellationToken cancellationToken);

    // EF
    
    public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<AuthorizationModel>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<AuthorizationModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<AuthorizationModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    // Supported getters/setters

    public ValueTask<string?> GetApplicationIdAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => new(authorization.ApplicationId.ToString());

    public ValueTask SetApplicationIdAsync(AuthorizationModel authorization, string? identifier,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(identifier, out var guid))
        {
            throw new ArgumentException("Not a valid guid.", nameof(identifier));
        }
        
        authorization.ApplicationId = guid;
        return new ValueTask();
    }


    public ValueTask<DateTimeOffset?> GetCreationDateAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => new(authorization.CreationDate);
    
    public ValueTask SetCreationDateAsync(AuthorizationModel authorization, DateTimeOffset? date,
        CancellationToken cancellationToken)
    {
        authorization.CreationDate = date;
        return new ValueTask();
    }
    

    public ValueTask<string?> GetIdAsync(AuthorizationModel authorization, CancellationToken cancellationToken)
        => new(authorization.Id.ToString());



    public ValueTask<ImmutableArray<string>> GetScopesAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => new(authorization.Scopes);
    
    public ValueTask SetScopesAsync(AuthorizationModel authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        authorization.Scopes = scopes;
        return new ValueTask();
    }

    
    public ValueTask<string?> GetStatusAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => new(authorization.Status);
    
    public ValueTask SetStatusAsync(AuthorizationModel authorization, string? status, CancellationToken cancellationToken)
    {
        authorization.Status = status;
        return new ValueTask();
    }


    public ValueTask<string?> GetSubjectAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => new(authorization.Subject.ToString());
    
    public ValueTask SetSubjectAsync(AuthorizationModel authorization, string? subject, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(subject, out var guid))
        {
            throw new ArgumentException("Not a valid guid.", nameof(subject));
        }
        
        authorization.Subject = guid;
        return new ValueTask();
    }


    public ValueTask<string?> GetTypeAsync(AuthorizationModel authorization, CancellationToken cancellationToken)
        => new(authorization.Type);
    
    public ValueTask SetTypeAsync(AuthorizationModel authorization, string? type, CancellationToken cancellationToken)
    {
        authorization.Type = type;
        return new ValueTask();
    }
    
    // Unsupported getters/setters

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(AuthorizationModel authorization,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetPropertiesAsync(AuthorizationModel authorization,
        ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}
