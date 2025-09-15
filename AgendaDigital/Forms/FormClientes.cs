using AgendaDigital.Data;
using AgendaDigital.Models;
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
    /// Formulario para la gestión de clientes.
    /// Permite registrar, editar, eliminar y listar clientes.
    /// </summary>
    public partial class FormClientes : Form
    {

        // Almacena el ID del cliente seleccionado en el grid.
        private int? clienteSeleccionadoId = null;
        public FormClientes()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario.
        /// Configura el combo de estados y carga la lista de clientes.
        /// </summary>
        private void FormClientes_Load(object sender, EventArgs e)
        {
            cmbEstado.Items.Clear();
            var estados = new[]
            {
                new { Valor = true, Texto = "Activo" },
                new { Valor = false, Texto = "Inactivo" }
            };
            cmbEstado.DataSource = estados;
            cmbEstado.ValueMember = "Valor";
            cmbEstado.DisplayMember = "Texto";
            cmbEstado.SelectedIndex = 0;
            CargarClientes();
        }

        /// <summary>
        /// Obtiene la lista de clientes desde la base de datos
        /// y la muestra en el DataGridView.
        /// </summary>
        private void CargarClientes()
        {
            using (var db = new AppDb())
            {
                dgvClientes.DataSource = db.Clientes
                                .OrderBy(c => c.Nombre)
                                .Select(c => new
                                {
                                    c.IdCliente,
                                    c.Nombre,
                                    c.Telefono,
                                    c.Correo,
                                    c.Direccion,
                                    EstadoTexto = c.Estado ? "Activo" : "Inactivo",
                                    c.Estado
                                })
                                .ToList();
                dgvClientes.Columns["IdCliente"].Visible = false;
                dgvClientes.Columns["Estado"].Visible = false;
                dgvClientes.Columns["EstadoTexto"].HeaderText = "Estado";
                dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvClientes.ClearSelection();
                clienteSeleccionadoId = null;
            }
            LimpiarFormulario();
        }

        // Limpia los campos del formulario y reinicia la selección.
        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtDireccion.Text = "";
            cmbEstado.SelectedIndex = 0;
            errorProvider1.Clear();
            btnGuardar.Enabled = true;
        }

        // Valida que los campos obligatorios del cliente estén completos.
        private bool ValidarDatos()
        {
            bool esValido = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider1.SetError(txtNombre, "El nombre es obligatorio.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                errorProvider1.SetError(txtTelefono, "El teléfono es obligatorio.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                errorProvider1.SetError(txtCorreo, "El correo es obligatorio.");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                errorProvider1.SetError(txtDireccion, "La dirección es obligatoria.");
                esValido = false;
            }
            return esValido;
        }

        /// <summary>
        /// Evento que se dispara al hacer clic en una celda del DataGridView.
        /// Carga los datos del cliente seleccionado en el formulario.
        /// </summary>
        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                clienteSeleccionadoId = (int)dgvClientes.Rows[e.RowIndex].Cells["IdCliente"].Value;

                txtNombre.Text = dgvClientes.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                txtTelefono.Text = dgvClientes.Rows[e.RowIndex].Cells["Telefono"].Value.ToString();
                txtCorreo.Text = dgvClientes.Rows[e.RowIndex].Cells["Correo"].Value.ToString();
                txtDireccion.Text = dgvClientes.Rows[e.RowIndex].Cells["Direccion"].Value.ToString();
                bool estado = (bool)dgvClientes.Rows[e.RowIndex].Cells["Estado"].Value;
                cmbEstado.SelectedValue = estado;
                btnGuardar.Enabled = false;
            }
        }

        // Prepara el formulario para registrar un nuevo cliente.
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            clienteSeleccionadoId = null;
        }

        // Guarda un nuevo cliente en la base de datos.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            using (var db = new AppDb())
            {
                var cliente = new Cliente
                {
                    Nombre = txtNombre.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    Estado = (bool)cmbEstado.SelectedValue
                };

                db.Clientes.Add(cliente);
                db.SaveChanges();
            }

            MessageBox.Show("Cliente agregado correctamente.");
            CargarClientes();
            LimpiarFormulario();
        }

        // Edita un cliente existente en la base de datos.
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (clienteSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un cliente de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidarDatos()) return;
            using (var db = new AppDb())
            {
                var cliente = db.Clientes.Find(clienteSeleccionadoId.Value);
                if (cliente == null) return;

                cliente.Nombre = txtNombre.Text.Trim();
                cliente.Telefono = txtTelefono.Text.Trim();
                cliente.Correo = txtCorreo.Text.Trim();
                cliente.Direccion = txtDireccion.Text.Trim();
                cliente.Estado = (bool)cmbEstado.SelectedValue;

                db.SaveChanges();
            }
            CargarClientes();
            LimpiarFormulario();
            MessageBox.Show("Cliente actualizado correctamente.");
        }

        // Elimina un cliente de la base de datos.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (clienteSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un cliente de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("¿Seguro que desea eliminar este cliente?",
                                          "Confirmar eliminación",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                using (var db = new AppDb())
                {
                    var cliente = db.Clientes.Find(clienteSeleccionadoId.Value);
                    if (cliente != null)
                    {
                        db.Clientes.Remove(cliente);
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Cliente eliminado permanentemente.");
                CargarClientes();
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
