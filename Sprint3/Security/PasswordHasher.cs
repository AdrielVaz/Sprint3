using System.Drawing;
using System.Security.Cryptography;

namespace Sprint3.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;// Tamanho do salt (em bytes) O salt é um valor aleatório usado para proteger contra ataques de tabela rainbow
        private const int KeySize = 32;//Tamanho do hash final (32 bytes = 256 bits)
        private const int Iterations = 100_000; // Número de iterações para o algoritmo PBKDF2, Quantidade de repetições do algoritmo

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize); //Gera um salt aleatório criptograficamente seguro
            byte[] key = Rfc2898DeriveBytes.Pbkdf2( //Usa o algoritmo PBKDF2 (padrão seguro para senhas)
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);
            /* password → senha original
               salt → valor aleatório
               Iterations → número de repetições
               SHA256 → algoritmo de hash
               KeySize → tamanho final
            */
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt;
            byte[] expectedKey;

            try
            {
                salt = Convert.FromBase64String(parts[0]);
                expectedKey = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                return false;
            }

            byte[] actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

    }
}