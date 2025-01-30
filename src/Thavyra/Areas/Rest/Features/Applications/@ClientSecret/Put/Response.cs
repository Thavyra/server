using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Applications.ClientSecret.Put;

[SchemaName("ClientSecretResponse")]
public class Response
{
    /// <summary>
    /// New client secret for the application. This value cannot be subsequently retrieved.
    /// </summary>
    public required string ClientSecret { get; set; }
}