namespace Thavyra.Rest.Features.Applications.Objectives.Post;

public class Request : ApplicationRequest
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}