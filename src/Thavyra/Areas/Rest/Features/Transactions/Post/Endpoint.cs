using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Transaction;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.Post;

public class Endpoint : Endpoint<Request, TransactionResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Transaction_Create> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Transaction_Create> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Post("/users/{User}/transactions");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException("Could not retrieve user.");
        }

        var createRequest = new Transaction_Create
        {
            ApplicationId = req.ApplicationId,
            SubjectId = user.Id,
            Description = req.Description.ValueOrDefault(),
            Amount = req.Amount
        };

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, createRequest,
            Security.Policies.Operation.Transaction.Send);

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

                    Description = transaction.Description,
                    Amount = transaction.Amount,

                    CreatedAt = transaction.CreatedAt
                }, cancellation: ct);
                break;
        }
    }
}