using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Update Application";
        Description = "Modify an application.";

        var example = Example.Application();
        
        ExampleRequest = new Request
        {
            Name = example.Name,
            Description = example.Description.Value,
        };
        
        Response(example: example);
        Response(404, "Application Not Found");
    }
}