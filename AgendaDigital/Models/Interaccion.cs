using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Models
{
    // Representa una interacción registrada entre un cliente y un usuario (colaborador).
    [Table("Interaccion")]
    public class Interaccion
    {

        // Identificador único de la interacción en la base de datos.
        [Key]
        [Column("IdInteraccion")]
        public int IdInteraccion { get; set; }

        // Identificador del cliente con el que se realizó la interacción.
        [Required]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        // Identificador del usuario (colaborador) que realizó la interacción.
        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        // Fecha y hora en que se realizó la interacción.
        [Required]
        [Column("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Descripción detallada de la interacción.
        [Required]
        [StringLength(300)]
        [Column("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        // Resultado o estado final de la interacción.
        [StringLength(150)]
        [Column("Resultado")]
        public string Resultado { get; set; }

        /// <summary>
        /// Cliente y usuario relacionado con esta interacción.
        /// Relación de navegación N:1.
        /// </summary>

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}
