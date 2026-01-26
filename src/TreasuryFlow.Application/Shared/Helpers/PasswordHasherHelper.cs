namespace TreasuryFlow.Application.Shared.Helpers
{
    using System.Security.Cryptography;

    public static class PasswordHasherHelper
    {
        public static string Hash(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100_000, HashAlgorithmName.SHA256);
            var salt = deriveBytes.Salt;
            var key = deriveBytes.GetBytes(32);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(key);
        }

        public static bool Verify(string password, string hash)
        {
            var parts = hash.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var key = Convert.FromBase64String(parts[1]);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var newKey = deriveBytes.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(key, newKey);
        }
    }

}
