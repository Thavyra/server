namespace Thavyra.Rest.Features.Applications.PutIcon;

public class Request : ApplicationRequest
{
    public IFormFile Icon { get; set; } = null!;
}