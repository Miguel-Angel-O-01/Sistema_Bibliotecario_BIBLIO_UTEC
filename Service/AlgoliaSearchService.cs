using Algolia.Search.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Services
{
    public class AlgoliaSearchService
    {
        private readonly SearchClient _client;
        private readonly string _indexName = "catalogo_libros";

        public AlgoliaSearchService(IConfiguration configuration)
        {
            var appId = configuration["ApiSettings:AlgoliaAppId"];
            var apiKey = configuration["ApiSettings:AlgoliaAdminApiKey"];

            // Configuración oficial para la versión moderna de Algolia
            _client = new SearchClient(new SearchConfig(appId, apiKey));
        }

        // Asegúrate de agregar 'string portadaUrl' a los parámetros
        public async Task IndexarLibroAsync(int id, string titulo, string autor, string isbn, string categoria, string portadaUrl)
        {
            // Usamos un diccionario compatible para serializar hacia Algolia
            var registroAlgolia = new Dictionary<string, string>
            {
                { "objectID", id.ToString() },
                { "Titulo", titulo },
                { "Autor", autor },
                { "Isbn", isbn },
                { "Categoria", categoria },
                { "PortadaUrl", portadaUrl ?? "" } // AGREGAMOS LA PORTADA AQUÍ
            };

            // Se envía el nombre del índice directamente en el método del cliente
            await _client.SaveObjectAsync(_indexName, registroAlgolia);
        }

        public async Task EliminarLibroIndexAsync(int id)
        {
            await _client.DeleteObjectAsync(_indexName, id.ToString());
        }
    }
}