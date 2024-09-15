using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Patch;

public class Request : RequestWithAuthentication
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}