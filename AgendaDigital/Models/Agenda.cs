using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaDigital.Models
{
    /// <summary>
    /// Representa una agenda digital en memoria, que agrupa clientes,
    /// interacciones y recordatorios. 
    /// No está mapeada a una tabla en la base de datos.
    /// </summary>
    [NotMapped]
    public class Agenda
    {
        // Lista de clientes en la agenda.
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();

        // Lista de interacciones en la agenda.
        public List<Interaccion> Interacciones { get; set; } = new List<Interaccion>();

        // Lista de recordatorios en la agenda.
        public List<Recordatorio> Recordatorios { get; set; } = new List<Recordatorio>();
    }
}
