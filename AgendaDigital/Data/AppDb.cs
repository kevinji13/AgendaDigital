using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Data
{
    public class AppDb : DbContext
    {
        // Se conecta usando la cadena de conexión llamada "AppDb" en el archivo de configuración.
        public AppDb() : base("name=AppDb")
        {
            // BD existente: desactivar cualquier initializer
            Database.SetInitializer<AppDb>(null);
        }

        // Tabla de usuarios registrados en el sistema.
        public DbSet<Models.Usuario> Usuarios { get; set; }

        // Tabla de clientes de la empresa.
        public DbSet<Models.Cliente> Clientes { get; set; }

        // Tabla que registra las interacciones realizadas con cada cliente.
        public DbSet<Models.Interaccion> Interacciones { get; set; }

        // Tabla de recordatorios asociados a los clientes o interacciones.
        public DbSet<Models.Recordatorio> Recordatorios { get; set; }
    }
}
