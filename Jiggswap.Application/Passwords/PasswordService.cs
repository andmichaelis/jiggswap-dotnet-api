using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JiggswapApi.Services
{
    public static class PasswordService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA512);
            var salt = Convert.ToBase64String(algorithm.Salt);
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));

            return $"{Iterations}.{salt}.{key}";
        }

        public static bool VerifyPassword(string hash, string password)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (password == null) throw new ArgumentNullException(nameof(password));

            var parts = hash.Split('.');

            var iterations = Convert.ToInt32(parts[0], CultureInfo.InvariantCulture);
            var hashSalt = Convert.FromBase64String(parts[1]);
            var hashKey = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(password, hashSalt, iterations, HashAlgorithmName.SHA512);
            var keyToCheck = algorithm.GetBytes(KeySize);

            return keyToCheck.SequenceEqual(hashKey);
        }
    }
}
