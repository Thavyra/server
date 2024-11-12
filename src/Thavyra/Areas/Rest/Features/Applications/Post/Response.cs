using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Post;

public class Response : ApplicationResponse
{
    public JsonOptional<string> ClientSecret { get; set; }
}