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
    /// Formulario para la gestión de recordatorios asociados a clientes.
    /// Permite crear, editar, eliminar y visualizar recordatorios.
    /// </summary>
    public partial class FormRecordatorios : Form
    {

        // Almacena el ID del recordatorio seleccionado en la grid.
        private int? recordatorioSeleccionadoID = null;
        public FormRecordatorios()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario.
        /// Carga la lista de clientes, estados disponibles y los datos iniciales.
        /// </summary>
        private void FormRecordatorios_Load(object sender, EventArgs e)
        {
            CargarClientes();
            CargarEstados();
            CargarDatos();
        }

        // Carga los clientes disponibles en el combo.
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

        // Carga los estados disponibles para los recordatorios.
        private void CargarEstados()
        {
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Pendiente");
            cmbEstado.Items.Add("Cumplido");
            cmbEstado.SelectedIndex = 0;
        }

        // Carga los recordatorios desde la base de datos y los muestra en el DataGridView.
        private void CargarDatos()
        {
            using (var db = new AppDb())
            {
                dgvRecordatorios.DataSource = db.Recordatorios
                    .OrderByDescending(r => r.FechaRecordatorio)
                    .Select(r => new
                    {
                        r.IdRecordatorio,
                        Cliente = r.Cliente.Nombre,
                        r.FechaRecordatorio,
                        r.Estado,
                        r.Nota
                    })
                    .ToList();
                dgvRecordatorios.Columns["IdRecordatorio"].Visible = false;
                dgvRecordatorios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvRecordatorios.ClearSelection();
                recordatorioSeleccionadoID = null;
            }

            LimpiarFormulario();
        }

        // Limpia los campos del formulario y reinicia la selección.
        private void LimpiarFormulario()
        {
            cmbCliente.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Now;
            txtNota.Text = "";
            recordatorioSeleccionadoID = null;
            btnGuardar.Enabled = true;
        }

        // Valida que los campos obligatorios estén completos.
        private bool ValidarDatos()
        {
            bool esValido = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtNota.Text))
            {
                errorProvider1.SetError(txtNota, "El resultado es obligatorio.");
                esValido = false;
            }
            return esValido;
        }

        // Prepara el formulario para registrar un nuevo recordatorio.
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            recordatorioSeleccionadoID = null;
        }

        // Guarda un nuevo recordatorio en la base de datos.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            using (var db = new AppDb())
            {
                var recordatorio = new Recordatorio
                {
                    IdCliente = (int)cmbCliente.SelectedValue,
                    FechaRecordatorio = dtpFecha.Value,
                    Estado = cmbEstado.SelectedItem.ToString(),
                    Nota = txtNota.Text.Trim()
                };

                db.Recordatorios.Add(recordatorio);
                db.SaveChanges();
            }

            MessageBox.Show("Recordatorio agregado correctamente.");
            CargarDatos();
            LimpiarFormulario();
        }

        // Edita el recordatorio seleccionado en la base de datos.
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (recordatorioSeleccionadoID == null)
            {
                MessageBox.Show("Seleccione un recordatorio de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidarDatos()) return;
            using (var db = new AppDb())
            {
                var recordatorio = db.Recordatorios.Find(recordatorioSeleccionadoID.Value);
                if (recordatorio == null) return;

                recordatorio.IdCliente = (int)cmbCliente.SelectedValue;
                recordatorio.FechaRecordatorio = dtpFecha.Value;
                recordatorio.Estado = cmbEstado.SelectedItem.ToString();
                recordatorio.Nota = txtNota.Text.Trim();

                db.SaveChanges();
            }
            CargarDatos();
            LimpiarFormulario();
            MessageBox.Show("Recordatorio actualizado correctamente.");
        }

        // Elimina un recordatorio existente de la base de datos.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (recordatorioSeleccionadoID == null)
            {
                MessageBox.Show("Seleccione un recordatorio de la lista.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("¿Seguro que desea eliminar este recordatorio?",
                                          "Confirmar eliminación",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                using (var db = new AppDb())
                {
                    var recordatorio = db.Recordatorios.Find(recordatorioSeleccionadoID.Value);
                    if (recordatorio != null)
                    {
                        db.Recordatorios.Remove(recordatorio);
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Recordatorio eliminado permanentemente.");
                CargarDatos();
                LimpiarFormulario();
            }
        }

        // Cierra el formulario actual.
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Evento al hacer clic en una celda del DataGridView.
        /// Carga los datos del recordatorio seleccionado en el formulario.
        /// </summary>
        private void dgvRecordatorios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                recordatorioSeleccionadoID = (int)dgvRecordatorios.Rows[e.RowIndex].Cells["IdRecordatorio"].Value;

                cmbCliente.Text = dgvRecordatorios.Rows[e.RowIndex].Cells["Cliente"].Value.ToString();
                dtpFecha.Value = (DateTime)dgvRecordatorios.Rows[e.RowIndex].Cells["FechaRecordatorio"].Value;
                cmbEstado.Text = dgvRecordatorios.Rows[e.RowIndex].Cells["Estado"].Value.ToString();
                txtNota.Text = dgvRecordatorios.Rows[e.RowIndex].Cells["Nota"].Value.ToString();
                btnGuardar.Enabled = false;
            }
        }
    }
}
