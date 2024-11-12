using System.Security.Cryptography;
using System.Text;

namespace Thavyra.Data.Security;

public class Secret
{
    private readonly string _secret;
    private static readonly char[] Chars =
        "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890".ToCharArray();

    private Secret(string secret)
    {
        _secret = secret;
    }

    public override string ToString()
    {
        return _secret;
    }

    public static Secret NewSecret(int length)
    {
        byte[] buffer = new byte[4 * length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            uint position = BitConverter.ToUInt32(buffer, i * 4);
            long index = position % Chars.Length;

            result.Append(Chars[index]);
        }

        return new Secret(result.ToString());
    }
}