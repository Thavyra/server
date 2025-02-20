using Thavyra.Rest.Documentation;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Patch;

[SchemaName("UpdateApplicationRequest")]
public class Request : ApplicationRequest
{
    public JsonOptional<string> Name { get; set; }
    public JsonOptional<string?> Description { get; set; }
}