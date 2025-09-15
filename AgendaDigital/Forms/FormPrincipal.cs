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
    /// Formulario principal de la aplicación.
    /// Presenta un dashboard con clientes activos, interacciones recientes
    /// y recordatorios pendientes. También contiene el menú de navegación.
    /// </summary>
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario principal.
        /// Muestra el usuario logueado en la barra de estado,
        /// habilita el menú de administración según el rol
        /// y carga la información inicial del dashboard.
        /// </summary>
        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // Bienvenida en la StatusStrip
            toolStripStatusLabel1.Text = $"Bienvenido, {SesionActual.Nombre} (Rol: {SesionActual.Rol})";

            // Menú de administración solo visible si es Admin
            administraciónToolStripMenuItem.Visible = SesionActual.EsAdministrador;

            // Cargar datos iniciales del dashboard
            CargarDashboard();
        }

        /// <summary>
        /// Carga datos de clientes activos, interacciones recientes y recordatorios pendientes
        /// desde la base de datos y los muestra en el dashboard.
        /// </summary>
        private void CargarDashboard()
        {
            using (var db = new AppDb())
            {
                var agenda = new Agenda
                {
                    Clientes = db.Clientes.Where(c => c.Estado).ToList(),
                    Interacciones = db.Interacciones
                        .Include("Cliente")
                        .Include("Usuario")
                        .OrderByDescending(i => i.Fecha)
                        .Take(10)
                        .ToList(),
                    Recordatorios = db.Recordatorios
                        .Include("Cliente")
                        .Where(r => r.Estado == "Pendiente")
                        .OrderBy(r => r.FechaRecordatorio)
                        .Take(10)
                        .ToList()
                };

                // Mostrar cantidad de clientes activos
                lblClientes.Text = $"Clientes activos: {agenda.Clientes.Count}";

                // Interacciones recientes
                dgvInteracciones.DataSource = agenda.Interacciones
                    .Select(i => new
                    {
                        Fecha = i.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        Cliente = i.Cliente.Nombre,
                        Usuario = i.Usuario.Nombre,
                        i.Descripcion,
                        i.Resultado
                    })
                    .ToList();

                // Recordatorios pendientes
                dgvRecordatorios.DataSource = agenda.Recordatorios
                    .Select(r => new
                    {
                        Fecha = r.FechaRecordatorio.ToString("dd/MM/yyyy"),
                        Cliente = r.Cliente.Nombre,
                        r.Nota,
                        r.Estado
                    })
                    .ToList();
            }
            dgvRecordatorios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvInteracciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        // Abre el formulario de gestión de clientes.
        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormClientes();
            form.ShowDialog();
            CargarDashboard();
        }

        // Abre el formulario de gestión de interacciones.
        private void interaccionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormInteracciones();
            form.ShowDialog();
            CargarDashboard();
        }

        // Abre el formulario de gestión de recordatorios.
        private void recordatoriosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormRecordatorios();
            form.ShowDialog();
            CargarDashboard();
        }

        // Abre el formulario de gestión de usuarios (solo para administradores).
        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormUsuarios();
            form.ShowDialog();
        }

        // Cierra la sesión actual y vuelve al formulario de login.
        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Desea cerrar la sesión?","Confirmar cierre de sesión",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SesionActual.CerrarSesion();
                this.Hide();
                var login = new FormLogin();
                login.Show();
            }
        }

        // Confirma y cierra la aplicación.
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea salir de la aplicación?", "Confirmar salida",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Asegura que al cerrar el formulario principal, se cierre toda la aplicación.
        private void FormPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
