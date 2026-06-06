using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public required string Email { get; set; }

        // ESTAS SON LAS DOS LÍNEAS QUE LE FALTAN A TU MODELO
        public required string Carnet { get; set; }
        public required string Password { get; set; }

        // La lista inicializada para evitar el otro error
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}