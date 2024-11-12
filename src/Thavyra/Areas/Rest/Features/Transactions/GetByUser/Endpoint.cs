using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Transaction;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.GetByUser;

public class Endpoint : Endpoint<UserRequest, List<TransactionResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Transaction_GetByUser> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Transaction_GetByUser> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Get("/users/{User}/transactions");
        Summary(x =>
        {
            x.Summary = "Get User Transactions";
        });
        Description(x => x.AutoTagOverride("Transactions"));
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, user,
                Security.Policies.Operation.User.ReadTransactions);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var collectRequest = new Transaction_GetByUser
        {
            UserId = user.Id
        };
        
        var response = await _client.GetResponse<Multiple<Transaction>>(collectRequest, ct);
        
        await SendAsync(response.Message.Items.Select(transaction => new TransactionResponse
        {
            IsTransfer = transaction.IsTransfer,
            
            Id = transaction.Id,
            ApplicationId = transaction.ApplicationId,
            SubjectId = transaction.SubjectId,
            RecipientId = transaction.RecipientId ?? default(JsonOptional<Guid>),
            
            Description = transaction.Description,
            Amount = transaction.Amount,
            
            CreatedAt = transaction.CreatedAt
        }).ToList(), cancellation: ct);
    }
}