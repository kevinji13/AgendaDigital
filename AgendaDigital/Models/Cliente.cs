using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Models
{
    // Representa a un cliente dentro de la agenda digital.
    [Table("Cliente")]
    public class Cliente
    {
        // Identificador único del cliente en la base de datos.
        [Key]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        // Nombre completo del cliente.
        [Required]
        [StringLength(150)]
        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Número telefónico de contacto del cliente.
        [StringLength(20)]
        [Column("Telefono")]
        public string Telefono { get; set; }

        // Correo electrónico del cliente.
        [StringLength(100)]
        [Column("Correo")]
        public string Correo { get; set; }

        // Dirección física del cliente.
        [StringLength(200)]
        [Column("Direccion")]
        public string Direccion { get; set; }

        // Estado del cliente (activo/inactivo).
        [Column("Estado")]
        public bool Estado { get; set; } = true;

        /// <summary>
        /// Lista de recordatorios e interacciones asociadas al cliente.
        /// Relación de navegación 1:N.
        /// </summary>

        public virtual ICollection<Interaccion> Interacciones { get; set; } = new HashSet<Interaccion>();
        public virtual ICollection<Recordatorio> Recordatorios { get; set; } = new HashSet<Recordatorio>();
    }
}
