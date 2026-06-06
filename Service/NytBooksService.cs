using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class NytBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NytBooksService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiSettings:NytApiKey"] ?? string.Empty;
        }

        public async Task<JArray?> ObtenerBestsellersAsync()
        {
            string url = $"https://nytimes.com{_apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);

                // Mapeo oficial: Extrae directamente la lista del nodo "results" -> "books"
                var booksArray = json["results"]?["books"] as JArray;
                return booksArray;
            }
            catch
            {
                return null;
            }

        }
    }
}
