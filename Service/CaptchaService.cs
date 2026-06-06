using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class CaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        // El constructor recibe el HttpClient e inyecta las llaves de appsettings.json de forma automática
        public CaptchaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["ApiSettings:ReCaptchaSecretKey"] ?? string.Empty;
        }

        // ¡ESTE ES EL MÉTODO QUE LE FALTA A TU CÓDIGO!
        public async Task<bool> ValidarTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                // Endpoint oficial de Google para comprobar si la respuesta de la casilla es válida
                string url = $"https://google.com{_secretKey}&response={token}";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Google nos devuelve un JSON con un campo booleano llamado "success"
                return json["success"]?.Value<bool>() ?? false;
            }
            catch
            {
                // Si hay algún problema de conexión con Google, denegamos el acceso por seguridad
                return false;
            }
        }
    }
}
