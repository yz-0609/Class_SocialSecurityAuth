using System;
using System.Security.Cryptography;
namespace SocialSecurityAuth
{
    public static class PasswordHasher
    {
        public const int Iterations = 120000;
        public static byte[] NewSalt()
        {
            byte[] salt = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            return salt;
        }
        public static byte[] Hash(string password, byte[] salt)
        {
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations)) return derive.GetBytes(64);
        }
        public static bool Verify(string password, byte[] salt, byte[] expected)
        {
            if (salt == null || expected == null || salt.Length != 32 || expected.Length != 64) return false;
            byte[] actual = Hash(password ?? "", salt);
            int difference = 0;
            for (int i = 0; i < actual.Length; i++) difference |= actual[i] ^ expected[i];
            return difference == 0;
        }
    }
}
