namespace Thavyra.Rest.Features.Applications.Icon.Put;

public class Request : ApplicationRequest
{
    /// <summary>
    /// The image file use as the icon.
    /// </summary>
    public IFormFile Icon { get; set; } = null!;
}