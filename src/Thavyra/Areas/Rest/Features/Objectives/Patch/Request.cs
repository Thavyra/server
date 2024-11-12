using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Patch;

public class Request : RequestWithAuthentication
{
    public Guid Id { get; set; }
    public JsonOptional<string> Name { get; set; }
    public JsonOptional<string> DisplayName { get; set; }
}