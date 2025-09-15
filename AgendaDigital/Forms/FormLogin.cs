using AgendaDigital.Data;
using AgendaDigital.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgendaDigital.Forms
{
    // Formulario de inicio de sesión de la aplicación.
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento ejecutado al presionar el botón de inicio de sesión.
        /// Valida los campos ingresados, comprueba el usuario en la base de datos
        /// y si es correcto, guarda la sesión y abre el formulario principal.
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuarioIngresado = txtUsuario.Text.Trim();
            string passwordIngresado = txtPassword.Text;

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(usuarioIngresado) || string.IsNullOrEmpty(passwordIngresado))
            {
                MessageBox.Show("Debe ingresar usuario y contraseña.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Buscar usuario por nombre de usuario
            using (var db = new AppDb())
            {
                var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuarioIngresado);

                if (usuario == null)
                {
                    MessageBox.Show("Usuario no encontrado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar la contraseña ingresada
                bool valido = PasswordHasher.Verify(passwordIngresado, usuario.PasswordHash);

                if (!valido)
                {
                    MessageBox.Show("Contraseña incorrecta.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Guardar en la sesión
                SesionActual.IdUsuario = usuario.IdUsuario;
                SesionActual.NombreUsuario = usuario.NombreUsuario;
                SesionActual.Nombre = usuario.Nombre;
                SesionActual.Rol = usuario.Rol;

                MessageBox.Show($"Bienvenido {SesionActual.Nombre} ({SesionActual.Rol})",
                    "Ingreso correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Abrir formulario principal
                this.Hide();
                var formPrincipal = new FormPrincipal(); // Menú Pricipal
                formPrincipal.Show();
            }
        }

        /// <summary>
        /// Evento ejecutado al presionar el botón de salir.
        /// Cierra la aplicación por completo.
        /// </summary>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
