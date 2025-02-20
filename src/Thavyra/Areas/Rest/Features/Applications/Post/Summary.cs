using FastEndpoints;
using Thavyra.Rest.Documentation;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Post;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Create Application";
        Description = "Create a new application.";

        var example = Example.Application();

        ExampleRequest = new Request
        {
            OwnerId = example.OwnerId,
            Name = example.Name,
            Description = example.Description.Value!,
            Type = example.IsConfidential.Value ? "web" : "native"
        };
        
        Response(201, "Success", example: example);
    }
}