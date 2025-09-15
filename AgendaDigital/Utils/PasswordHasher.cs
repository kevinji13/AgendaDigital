using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Utils
{
    // Clase estática para manejar el hashing y verificación de contraseñas.
    public static class PasswordHasher
    {
        // Formato almacenado: iterations.saltBase64.hashBase64
        public static string Hash(string password, int iterations = 10000)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[16];
                rng.GetBytes(salt);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    var hash = pbkdf2.GetBytes(32);
                    return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
                }
            }
        }
        // Verifica si una contraseña coincide con un hash previamente almacenado.
        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored) || password == null) return false;

            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            // Obtener iteraciones, salt y hash almacenado
            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);

            // Recalcular el hash con los mismos parámetros
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                var computed = pbkdf2.GetBytes(32);
                return computed.SequenceEqual(storedHash);
            }
        }
    }
}
