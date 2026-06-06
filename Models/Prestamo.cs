using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaMaxDevolucion { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string? Estado { get; set; }

        // ! ESTAS PROPIEDADES DEBEN EXISTIR EXACTAMENTE ASÍ:
        public virtual Libro? Libro { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
