using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Transaction;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.PostTransfer;

public class Endpoint : Endpoint<Request, TransactionResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Transfer_Create> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Transfer_Create> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Post("/users/{User}/transactions/transfer");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var createRequest = new Transfer_Create
        {
            ApplicationId = req.ApplicationId,
            SubjectId = user.Id,
            RecipientId = req.RecipientId,
            Description = req.Description.ValueOrDefault(),
            Amount = req.Amount
        };
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, createRequest,
            Security.Policies.Operation.Transaction.Transfer);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        Response response = await _client.GetResponse<Transaction, InsufficientBalance>(createRequest, ct);
        
        switch (response)
        {
            case (_, InsufficientBalance):
                AddError(r => r.Amount, "User does not have sufficient balance for the specified amount.");
                await SendErrorsAsync(cancellation: ct);
                return;
            case (_, Transaction transaction):
                await SendCreatedAtAsync<Get.Endpoint>(new { Id = transaction.Id.ToString() }, new TransactionResponse
                {
                    IsTransfer = transaction.IsTransfer,

                    Id = transaction.Id,
                    ApplicationId = transaction.ApplicationId,
                    SubjectId = transaction.SubjectId,
                    RecipientId = transaction.RecipientId ?? default(JsonOptional<Guid>),

                    Description = transaction.Description,
                    Amount = transaction.Amount,

                    CreatedAt = transaction.CreatedAt
                }, cancellation: ct);
                break;
        }
    }
}