using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Models
{
    // Representa un recordatorio asociado a un cliente.
    [Table("Recordatorio")]
    public class Recordatorio
    {
        // Identificador único del recordatorio en la base de datos.
        [Key]
        [Column("IdRecordatorio")]
        public int IdRecordatorio { get; set; }

        // Identificador del cliente asociado al recordatorio.
        [Required]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        // Fecha y hora del recordatorio.
        [Required]
        [Column("FechaRecordatorio")]
        public DateTime FechaRecordatorio { get; set; }

        // Estado del recordatorio (Pendiente, Completado)
        [StringLength(20)]
        [Column("Estado")]
        public string Estado { get; set; } = "Pendiente";

        // Nota o descripción del recordatorio.
        [StringLength(200)]
        [Column("Nota")]
        public string Nota { get; set; }

        /// <summary>
        /// Cliente al que pertenece este recordatorio.
        /// Relación de navegación N:1.
        /// </summary>
        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }
    }
}
