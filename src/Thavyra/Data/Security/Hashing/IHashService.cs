namespace Thavyra.Data.Security.Hashing;

/// <summary>
/// Cryptographic password/secret hashing service.
/// </summary>
public interface IHashService
{
    Task<string> HashAsync(string plaintext);

    Task<HashResult> CheckAsync(string plaintext, string hash);
}