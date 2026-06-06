using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;


namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class StripeService
    {
        public StripeService(IConfiguration configuration)
        {
            // Se inicializa la llave secreta de pruebas globalmente
            StripeConfiguration.ApiKey = configuration["ApiSettings:StripeSecretKey"];
        }

        public async Task<PaymentIntent> CrearIntencionPagoAsync(decimal montoCentavos, string emailEstudiante, int multaId)
        {
            var options = new PaymentIntentCreateOptions
            {
                // Stripe procesa en centavos (Ej: $5.000 se envían como 500)
                Amount = (long)(montoCentavos * 100),
                Currency = "usd",
                ReceiptEmail = emailEstudiante,
                PaymentMethodTypes = new List<string> { "card" },
                // Metadatos para vincular la transacción con tu base de datos SQLite
                Metadata = new Dictionary<string, string>
            {
                { "MultaId", multaId.ToString() }
            }
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }
    }
}
