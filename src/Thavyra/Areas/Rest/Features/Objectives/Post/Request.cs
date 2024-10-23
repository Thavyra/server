using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Request : ApplicationRequest
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}