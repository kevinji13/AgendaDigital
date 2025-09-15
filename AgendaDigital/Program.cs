using AgendaDigital.Data;
using AgendaDigital.Models;
using AgendaDigital.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgendaDigital
{
    // Clase principal de la aplicación.
    internal static class Program
    {
        [STAThread]
        // Método principal de la aplicación.
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicializar la base de datos
            using (var db = new AppDb())
            {
                var c = db.Database.Connection;
                if (c.State == System.Data.ConnectionState.Closed) c.Open();

                // Mensaje informativo al iniciar
                MessageBox.Show($"Servidor: {c.DataSource}\nBD: {c.Database}\nUsuarios: {db.Usuarios.Count()}");

                db.Database.Initialize(false);
                db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                // Sembrar usuario admin si la tabla está vacía
                if (!db.Usuarios.Any())
                {
                    db.Usuarios.Add(new Usuario
                    {
                        Nombre = "Administrador General",
                        Rol = "Admin",
                        NombreUsuario = "admin",
                        PasswordHash = PasswordHasher.Hash("admin123")
                    });
                    db.SaveChanges();
                }
            }

            // Lanzar el formulario de inicio de sesión
            Application.Run(new Forms.FormLogin());
        }
    }
}
