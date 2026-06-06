using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public class Multa
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Prestamo")]
        public int PrestamoId { get; set; }
        public virtual Prestamo? Prestamo { get; set; }

        public decimal Monto { get; set; }
        public bool EstaPagada { get; set; }
        public string? StripePaymentIntentId { get; set; } // ID de transacción de Stripe
    }
}
