using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class BrevoEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public BrevoEmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiSettings:BrevoApiKey"] ?? string.Empty;
        }

        public async Task<bool> EnviarNotificacionPrestamoAsync(string emailDestino, string nombreUsuario, string tituloLibro, DateTime fechaVencimiento)
        {
            var emailData = new
            {
                sender = new { name = "Biblioteca UTÈC", email = "no-reply@biblioutec.com" },
                to = new[] { new { email = emailDestino, name = nombreUsuario } },
                subject = "Confirmación de Reserva de Libro",
                htmlContent = $@"
                    <div style='font-family: Arial; background-color: #f4f4f4; padding: 20px; border-radius: 8px;'>
                        <h2 style='color: #0056b3;'>📚 Sistema Bibliotecario BiblioUTEC</h2>
                        <p>Hola <strong>{nombreUsuario}</strong>,</p>
                        <p>Tu reserva para el libro <strong>{tituloLibro}</strong> se procesó con éxito.</p>
                        <p>Recuerda pasar a recogerlo y devolverlo antes del: <span style='color: red; font-weight: bold;'>{fechaVencimiento.ToShortDateString()}</span></p>
                    </div>"
            };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://brevo.com");
                request.Headers.Add("api-key", _apiKey);
                request.Content = new StringContent(JsonConvert.SerializeObject(emailData), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
