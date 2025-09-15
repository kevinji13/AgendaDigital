using AgendaDigital.Data;
using AgendaDigital.Models;
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
    /// <summary>
    /// Formulario para la gestión de usuarios del sistema.
    /// Permite registrar, editar, eliminar y listar usuarios.
    /// </summary>
    public partial class FormUsuarios : Form
    {

        // Almacena el ID del usuario seleccionado en el grid.
        private int? usuarioSeleccionadoId = null;
        public FormUsuarios()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario.
        /// Carga los roles disponibles y los datos de los usuarios.
        /// </summary>
        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            CargarRoles();
            CargarDatos();
        }

        // Carga la lista de roles disponibles en el combo.
        private void CargarRoles()
        {
            cmbRol.Items.Clear();
            cmbRol.Items.Add("Admin");
            cmbRol.Items.Add("Usuario");
            cmbRol.SelectedIndex = 0;
        }

        // Obtiene los usuarios desde la base de datos y los muestra en el DataGridView.
        private void CargarDatos()
        {
            using (var db = new AppDb())
            {
                dgvUsuarios.DataSource = db.Usuarios
                    .OrderBy(u => u.Nombre)
                    .Select(u => new
                    {
                        u.IdUsuario,
                        u.Nombre,
                        u.NombreUsuario,
                        u.Rol
                    })
                    .ToList();

                dgvUsuarios.Columns["IdUsuario"].Visible = false;
                dgvUsuarios.Columns["Nombre"].HeaderText = "Nombre";
                dgvUsuarios.Columns["NombreUsuario"].HeaderText = "Usuario";
                dgvUsuarios.Columns["Rol"].HeaderText = "Rol";

                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvUsuarios.ClearSelection();
                usuarioSeleccionadoId = null;
            }
            LimpiarFormulario();
        }

        // Limpia los campos del formulario y reinicia la selección.
        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtNombreUsuario.Text = "";
            txtPassword.Text = "";
            cmbRol.SelectedIndex = 0;
            usuarioSeleccionadoId = null;
            btnGuardar.Enabled = true;
        }

        /// <summary>
        /// Evento al hacer clic en una celda del DataGridView.
        /// Carga los datos del usuario seleccionado en el formulario.
        /// </summary>
        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                usuarioSeleccionadoId = (int)dgvUsuarios.Rows[e.RowIndex].Cells["IdUsuario"].Value;

                txtNombre.Text = dgvUsuarios.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                txtNombreUsuario.Text = dgvUsuarios.Rows[e.RowIndex].Cells["NombreUsuario"].Value.ToString();
                cmbRol.Text = dgvUsuarios.Rows[e.RowIndex].Cells["Rol"].Value.ToString();
                
                btnGuardar.Enabled = false;
            }
        }

        // Valida que los campos obligatorios estén completos.
        private bool ValidarDatos()
        {
            bool esValido = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider1.SetError(txtNombre, "El nombre es obligatorio.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                errorProvider1.SetError(txtNombreUsuario, "El nombre de usuario es obligatorio.");
                esValido = false;
            }

            if (usuarioSeleccionadoId == null && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                // La contraseña solo es obligatoria en creación.
                errorProvider1.SetError(txtPassword, "La contraseña es obligatoria.");
                esValido = false;
            }
            return esValido;
        }

        // Prepara el formulario para registrar un nuevo usuario.
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            usuarioSeleccionadoId = null;
        }

        // Guarda un nuevo usuario en la base de datos.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            using (var db = new AppDb())
            {
                var user = new Usuario
                {
                    Nombre = txtNombre.Text.Trim(),
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Rol = cmbRol.SelectedItem.ToString(),
                    PasswordHash = PasswordHasher.Hash(txtPassword.Text.Trim())
                };

                db.Usuarios.Add(user);
                db.SaveChanges();
            }
            MessageBox.Show("Usuario agregado correctamente.");
            CargarDatos();
            LimpiarFormulario();
        }

        // Edita un usuario existente en la base de datos.
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un usuario de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidarDatos()) return;
            using (var db = new AppDb())
            {
                var user = db.Usuarios.Find(usuarioSeleccionadoId.Value);
                if (user == null) return;

                user.Nombre = txtNombre.Text.Trim();
                user.NombreUsuario = txtNombreUsuario.Text.Trim();
                user.Rol = cmbRol.SelectedItem.ToString();
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    user.PasswordHash = PasswordHasher.Hash(txtPassword.Text.Trim());
                }

                db.SaveChanges();
            }
            CargarDatos();
            LimpiarFormulario();
            MessageBox.Show("Usuario actualizado correctamente.");
        }

        // Elimina un usuario de la base de datos.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un usuario de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("¿Seguro que desea eliminar este usuario?",
                                          "Confirmar eliminación",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                using (var db = new AppDb())
                {
                    var user = db.Usuarios.Find(usuarioSeleccionadoId.Value);
                    if (user != null)
                    {
                        db.Usuarios.Remove(user);
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Usuario eliminado permanentemente.");
                CargarDatos();
                LimpiarFormulario();
            }
        }

        // Cierra el formulario actual.
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
