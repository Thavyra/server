using Microsoft.Extensions.Options;
using BC = BCrypt.Net.BCrypt;

namespace Thavyra.Data.Security.Hashing;

public class BCryptHashService : IHashService
{
    private readonly BCryptOptions _options;

    public BCryptHashService(IOptions<BCryptOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task<string> HashAsync(string plaintext)
    {
        string hash = BC.HashPassword(plaintext, BC.GenerateSalt(_options.Rounds));

        return hash;
    }

    public async Task<HashResult> CheckAsync(string plaintext, string hash)
    {
        bool correct = BC.Verify(plaintext, hash);

        if (correct && BC.PasswordNeedsRehash(hash, _options.Rounds))
        {
            return new HashResult(correct, await HashAsync(plaintext));
        }
        
        return new HashResult(correct);
    }
}