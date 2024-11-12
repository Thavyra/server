using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Request : ApplicationRequest
{
    public JsonOptional<string> Name { get; set; }
    public JsonOptional<string?> Description { get; set; }
}