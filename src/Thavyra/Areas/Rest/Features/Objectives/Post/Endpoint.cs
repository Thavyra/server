using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Endpoint : Endpoint<Request, ObjectiveResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Objective_Create> _client;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Objective_Create> client)
    {
        _authorizationService = authorizationService;
        _client = client;
    }

    public override void Configure()
    {
        Post("/applications/{Application}/objectives");
        
        Description(x => x.AutoTagOverride("Scoreboard"));
        
        Summary(x =>
        {
            x.Summary = "Create Objective";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().Application is not { } application)
        {
            throw new InvalidOperationException();
        }
        
        var createRequest = new Objective_Create
        {
            ApplicationId = application.Id,
            Name = req.Name,
            DisplayName = req.DisplayName
        };
        
        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, createRequest, Security.Policies.Operation.Objective.Create);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _client.GetResponse<Objective>(createRequest, ct);

        await SendCreatedAtAsync<Get.Endpoint>(new { Id = response.Message.Id }, new ObjectiveResponse
        {
            Id = response.Message.Id,
            ApplicationId = response.Message.ApplicationId,
            Name = response.Message.Name,
            DisplayName = response.Message.DisplayName,
            CreatedAt = response.Message.CreatedAt
        }, cancellation: ct);
    }
}