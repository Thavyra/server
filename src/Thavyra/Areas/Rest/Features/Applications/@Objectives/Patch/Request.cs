using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Objectives.Patch;

public class Request : ApplicationRequest
{
    public Guid Id { get; set; }
    public JsonOptional<string> Name { get; set; }
    public JsonOptional<string> DisplayName { get; set; }
}