namespace bretts_services.Utilities;

public static class Hashing
{
    const int KEY_SIZE = 64;
    const int ITTERATIONS = 350000;

    public static byte[] Hash(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KEY_SIZE);

        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, ITTERATIONS, HashAlgorithmName.SHA512, KEY_SIZE);

        return hash;
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITTERATIONS, HashAlgorithmName.SHA512, KEY_SIZE);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
    }
}
