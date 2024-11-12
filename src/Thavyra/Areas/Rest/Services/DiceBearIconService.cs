using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Thavyra.Rest.Configuration;

namespace Thavyra.Rest.Services;

public class DiceBearIconService : IIconService
{
    private readonly HttpClient _httpClient;
    private readonly DiceBearOptions _options;

    public DiceBearIconService(HttpClient httpClient, IOptions<DiceBearOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<Stream> GetUrlIconAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public async Task<Stream> GetDefaultIconAsync(string? style = null, string? seed = null, int size = 128, CancellationToken cancellationToken = default)
    {
        var query = new QueryBuilder(new Dictionary<string, string>
        {
            ["size"] = size.ToString(),
        });

        if (!string.IsNullOrEmpty(seed))
        {
            query.Add("seed", seed);
        }

        var response = await _httpClient.GetAsync($"{_options.BaseUrl}/{style ?? _options.DefaultStyle ?? "initials"}/png{query.ToQueryString()}", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}