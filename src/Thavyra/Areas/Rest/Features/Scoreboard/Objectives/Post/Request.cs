using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scoreboard.Objectives.Post;

public class Request : RequestWithAuthentication
{
    public string Name { get; set; } = null!;
    public double Score { get; set; }
}