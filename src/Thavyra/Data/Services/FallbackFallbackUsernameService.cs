using Microsoft.Extensions.Options;
using Thavyra.Data.Configuration;

namespace Thavyra.Data.Services;

public class FallbackUsernameService : IFallbackUsernameService
{
    private readonly UserOptions _options;

    public FallbackUsernameService(IOptions<UserOptions> options)
    {
        _options = options.Value;
    }
    
    public Task<string> GenerateFallbackUsernameAsync()
    {
        var noun = Random.Shared.GetItems(_options.FallbackUsernames.Nouns.ToArray(), 1)[0];
        
        var adjective = Random.Shared.GetItems(_options.FallbackUsernames.Adjectives
            .Where(x => x.StartsWith(noun[0]))
            .ToArray(), 1)[0];

        var number = Random.Shared.Next(100, 999);

        return Task.FromResult($"{adjective}{noun}{number}");
    }
}