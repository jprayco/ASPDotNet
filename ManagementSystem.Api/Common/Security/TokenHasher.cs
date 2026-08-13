using System.Security.Cryptography;
using System.Text;

namespace ManagementSystem.Api.Common.Security;

public class TokenHasher
{
    public static string Hash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
