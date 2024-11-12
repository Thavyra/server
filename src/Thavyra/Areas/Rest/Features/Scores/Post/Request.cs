using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.Post;

public class Request : RequestWithAuthentication
{
    public Guid ObjectiveId { get; set; }
    public double Score { get; set; }
}