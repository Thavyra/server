using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Transaction;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.Get;

public class Endpoint : Endpoint<Request, TransactionResponse>
{
    private readonly IRequestClient<Transaction_GetById> _client;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IRequestClient<Transaction_GetById> client, IAuthorizationService authorizationService)
    {
        _client = client;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/transactions/{Id}");
        Summary(x =>
        {
            x.Summary = "Get Transaction";
        });
        Description(x => x.AutoTagOverride("Transactions"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response transactionResponse = await _client.GetResponse<Transaction, NotFound>(new Transaction_GetById
        {
            Id = req.Id
        }, ct);

        switch (transactionResponse)
        {
            case (_, NotFound):
                await SendNotFoundAsync(ct);
                return;
        }

        if (transactionResponse is not (_, Transaction transaction))
        {
            throw new InvalidOperationException("Could not retrieve transaction.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, transaction, Security.Policies.Operation.Transaction.Read);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        await SendAsync(new TransactionResponse
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
    }
}