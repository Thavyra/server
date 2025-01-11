namespace Thavyra.Rest.Features.Applications.Icon.Put;

public class Request : ApplicationRequest
{
    public IFormFile Icon { get; set; } = null!;
}