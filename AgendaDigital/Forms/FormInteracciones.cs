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
using System.Data.Entity;

namespace AgendaDigital.Forms
{
    /// <summary>
    /// Formulario para la gestión de interacciones entre usuarios y clientes.
    /// Permite registrar, editar, eliminar y visualizar interacciones.
    /// </summary>
    public partial class FormInteracciones : Form
    {

        // Almacena el ID de la interacción seleccionada en la grid.
        private int? interaccionSeleccionadaID = null;
        public FormInteracciones()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario.
        /// Configura combos, carga datos y muestra el usuario logueado.
        /// </summary>
        private void FormInteracciones_Load(object sender, EventArgs e)
        {
            CargarClientes();
            CargarTipos();
            CargarDatos();

            // Mostrar usuario actual en el campo txtUsuario
            txtUsuario.Text = SesionActual.Nombre;
            txtUsuario.ReadOnly = true;
        }

        // Carga la lista de clientes en el combo correspondiente.
        private void CargarClientes()
        {
            using (var db = new AppDb())
            {
                var clientes = db.Clientes
                                 .Select(c => new { c.IdCliente, c.Nombre })
                                 .ToList();

                cmbCliente.DataSource = clientes;
                cmbCliente.DisplayMember = "Nombre";
                cmbCliente.ValueMember = "IdCliente";
            }
        }

        // Carga los tipos de interacción disponibles en el combo.
        private void CargarTipos()
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("Llamada");
            cmbTipo.Items.Add("Correo");
            cmbTipo.Items.Add("Reunión");
            cmbTipo.Items.Add("WhatsApp");
            cmbTipo.SelectedIndex = 0;
        }

        // Carga las interacciones desde la base de datos y las muestra en el DataGridView.
        private void CargarDatos()
        {
            using (var db = new AppDb())
            {
                dgvInteracciones.DataSource = db.Interacciones
                    .OrderByDescending(i => i.Fecha)
                    .Select(i => new
                    {
                        i.IdInteraccion,
                        Cliente = i.Cliente.Nombre,
                        Usuario = i.Usuario.Nombre,
                        i.Fecha,
                        Tipo = i.Descripcion,
                        i.Resultado
                    })
                    .ToList();
                dgvInteracciones.Columns["IdInteraccion"].Visible = false;
                dgvInteracciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvInteracciones.ClearSelection();
                interaccionSeleccionadaID = null;
            }
            LimpiarFormulario();
        }

        // Limpia los campos del formulario y reinicia la selección.
        private void LimpiarFormulario()
        {
            cmbCliente.SelectedIndex = 0;
            cmbTipo.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Now;
            txtResultado.Text = "";
            interaccionSeleccionadaID = null;
            errorProvider1.Clear();
            btnGuardar.Enabled = true;
        }

        // Valida que los campos obligatorios estén completos.
        private bool ValidarDatos()
        {
            bool esValido = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtResultado.Text))
            {
                errorProvider1.SetError(txtResultado, "El resultado es obligatorio.");
                esValido = false;
            }
            return esValido;
        }

        /// <summary>
        /// Evento al hacer clic en una celda del DataGridView.
        /// Carga los datos de la interacción seleccionada en el formulario.
        /// </summary>
        private void dgvInteracciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                interaccionSeleccionadaID = (int)dgvInteracciones.Rows[e.RowIndex].Cells["IdInteraccion"].Value;

                cmbCliente.Text = dgvInteracciones.Rows[e.RowIndex].Cells["Cliente"].Value.ToString();
                txtUsuario.Text = dgvInteracciones.Rows[e.RowIndex].Cells["Usuario"].Value.ToString();
                dtpFecha.Value = (DateTime)dgvInteracciones.Rows[e.RowIndex].Cells["Fecha"].Value;
                cmbTipo.Text = dgvInteracciones.Rows[e.RowIndex].Cells["Tipo"].Value.ToString();
                txtResultado.Text = dgvInteracciones.Rows[e.RowIndex].Cells["Resultado"].Value.ToString();
                btnGuardar.Enabled = false;
            }
        }

        // Prepara el formulario para registrar una nueva interacción.
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            interaccionSeleccionadaID = null;
        }

        // Guarda una nueva interacción en la base de datos.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            using (var db = new AppDb())
            {
                var interaccion = new Interaccion
                {
                    IdCliente = (int)cmbCliente.SelectedValue,
                    IdUsuario = SesionActual.IdUsuario,
                    Fecha = dtpFecha.Value,
                    Descripcion = cmbTipo.SelectedItem.ToString(),
                    Resultado = txtResultado.Text.Trim()
                };

                db.Interacciones.Add(interaccion);
                db.SaveChanges();
            }

            MessageBox.Show("Interacción agregada correctamente.");
            CargarDatos();
            LimpiarFormulario();
        }

        // Edita la interacción seleccionada.
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (interaccionSeleccionadaID == null)
            {
                MessageBox.Show("Seleccione una interacción de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidarDatos()) return;
            using (var db = new AppDb())
            {
                var interaccion = db.Interacciones.Find(interaccionSeleccionadaID.Value);
                if (interaccion == null) return;

                interaccion.IdCliente = (int)cmbCliente.SelectedValue;
                interaccion.Fecha = dtpFecha.Value;
                interaccion.Descripcion = cmbTipo.SelectedItem.ToString();
                interaccion.Resultado = txtResultado.Text.Trim();

                db.SaveChanges();
            }
            CargarDatos();
            LimpiarFormulario();
            MessageBox.Show("Interacción actualizada correctamente.");
        }

        // Elimina la interacción seleccionada de la base de datos.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (interaccionSeleccionadaID == null)
            {
                MessageBox.Show("Seleccione una interacción de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("¿Seguro que desea eliminar esta interacción?",
                                          "Confirmar eliminación",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                using (var db = new AppDb())
                {
                    var interaccion = db.Interacciones.Find(interaccionSeleccionadaID.Value);
                    if (interaccion != null)
                    {
                        db.Interacciones.Remove(interaccion);
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Interacción eliminada permanentemente.");
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
