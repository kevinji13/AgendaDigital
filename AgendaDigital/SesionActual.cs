using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital
{
    // Clase estática que mantiene la información de la sesión actual del usuario.
    public static class SesionActual
    {
        // Identificador único del usuario en la base de datos.
        public static int IdUsuario { get; set; }

        // Nombre de usuario utilizado para el inicio de sesión.
        public static string NombreUsuario { get; set; } = string.Empty;

        // Nombre completo del usuario.
        public static string Nombre { get; set; } = string.Empty;
        public static string Rol { get; set; } = string.Empty;

        //Indica si el usuario actual es administrador.
        public static bool EsAdministrador => Rol.ToUpper() == "ADMIN";

        // Cierra la sesión actual.
        public static void CerrarSesion()
        {
            IdUsuario = 0;
            NombreUsuario = string.Empty;
            Nombre = string.Empty;
            Rol = string.Empty;
        }
    }
}

