using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Models
{
    // Representa a un usuario dentro del sistema.
    [Table("Usuario")]
    public class Usuario
    {
        // Identificador único del usuario en la base de datos.
        [Key]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        // Nombre completo del usuario.
        [Required]
        [StringLength(100)]
        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Rol asignado al usuario (ejemplo: ADMIN, USUARIO).
        [Required]
        [StringLength(50)]
        [Column("Rol")]
        public string Rol { get; set; } = "";

        // Nombre de usuario único para iniciar sesión.
        [Required]
        [StringLength(50)]
        [Column("NombreUsuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        // Hash de la contraseña para autenticación segura.
        [Required]
        [StringLength(255)]
        [Column("ContrasenaHash")]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Colección de interacciones realizadas por este usuario con los clientes.
        /// Relación de navegación 1:N.
        /// </summary>
        public virtual ICollection<Interaccion> Interacciones { get; set; } = new HashSet<Interaccion>();
    }
}
