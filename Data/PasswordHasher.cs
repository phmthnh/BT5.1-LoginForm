using System.Security.Cryptography;

namespace BT5_1_LoginForm.Data;

internal static class PasswordHasher
{
    internal const int Iterations = 600_000;

    internal static (byte[] Hash, byte[] Salt) Create(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return (Derive(password, salt, Iterations), salt);
    }

    internal static bool Verify(string password, byte[] hash, byte[] salt, int iterations)
    {
        if (hash.Length != 32 || salt.Length != 16 || iterations is < 100_000 or > 2_000_000)
            return false;

        var candidate = Derive(password, salt, iterations);
        return CryptographicOperations.FixedTimeEquals(candidate, hash);
    }

    private static byte[] Derive(string password, byte[] salt, int iterations) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);
}
