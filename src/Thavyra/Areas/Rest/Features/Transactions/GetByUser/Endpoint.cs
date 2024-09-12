using FastEndpoints;
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
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<UserRequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var collectRequest = new Transaction_GetByUser
        {
            UserId = user.Id
        };

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, collectRequest,
                Security.Policies.Operation.Transaction.Read);
        
        if (authorizationResult.Failure?.FailureReasons is { } reasons)
            foreach (var reason in reasons)
            {
                AddError(reason.Message);
            }

        if (authorizationResult.Failed())
        {
            await SendErrorsAsync(StatusCodes.Status403Forbidden, ct);
            return;
        }

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