namespace Thavyra.Rest.Services;

public interface IIconService
{
    Task<Stream> GetUrlIconAsync(string url, CancellationToken cancellationToken = default);
    Task<Stream> GetDefaultIconAsync(string? style = null, string? seed = null, int size = 128, CancellationToken cancellationToken = default);
}