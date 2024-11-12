using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Delete;

public class Request : RequestWithAuthentication
{
    public Guid Id { get; set; }
}