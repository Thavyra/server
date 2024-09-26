namespace Thavyra.Data.Security.Hashing;

public struct HashResult
{
    public HashResult(bool succeeded)
    {
        Succeeded = succeeded;
    }

    public HashResult(bool succeeded, string rehash)
    {
        Succeeded = succeeded;
        Rehash = rehash;
    }
    
    public bool Succeeded { get; }
    public string? Rehash { get; }
}