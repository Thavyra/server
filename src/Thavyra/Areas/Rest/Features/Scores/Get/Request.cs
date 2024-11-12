using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Scores.Get;

public class Request : RequestWithAuthentication
{
    public Guid Id { get; set; }
}