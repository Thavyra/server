using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Post;

public class Request : RequestWithAuthentication
{
    public JsonOptional<Guid> ObjectiveId { get; set; }
    public JsonOptional<string> ObjectiveName { get; set; } = null!;
    public double Score { get; set; }
}