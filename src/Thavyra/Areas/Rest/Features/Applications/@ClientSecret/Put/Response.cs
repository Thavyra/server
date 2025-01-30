namespace Thavyra.Rest.Features.Applications.ClientSecret.Put;

public class Response
{
    /// <summary>
    /// New client secret for the application. This value cannot be subsequently retrieved.
    /// </summary>
    public required string ClientSecret { get; set; }
}