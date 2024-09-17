namespace Thavyra.Rest.Features.Redirects;

public class RedirectResponse
{
    public required Guid Id { get; set; }
    public required Guid ApplicationId { get; set; }
    public required string Uri { get; set; }
    public required DateTime CreatedAt { get; set; }
}