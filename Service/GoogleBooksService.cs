using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleBooksService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiSettings:GoogleBooksApiKey"] ?? string.Empty;
        }

        public async Task<Libro?> BuscarPorIsbnAsync(string isbn)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}&key={_apiKey}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if ((int?)json["totalItems"] == 0 || json["items"] == null)
                {
                    return null;
                }

                var volumeInfo = json["items"]?[0]?["volumeInfo"];
                if (volumeInfo == null) return null;

                var nuevoLibro = new Libro
                {
                    Isbn = isbn,
                    Titulo = volumeInfo!["title"]?.ToString() ?? "Título desconocido",
                    Sinopsis = volumeInfo!["description"]?.ToString() ?? "Sin descripción disponible.",
                    PortadaUrl = volumeInfo["imageLinks"]?["thumbnail"]?.ToString() ?? "",

                    Autor = volumeInfo!["authors"] != null
                        ? string.Join(", ", volumeInfo!["authors"]!.ToObject<string[]>() ?? Array.Empty<string>())
                        : "Autor no registrado",

                    Categoria = volumeInfo!["categories"] != null
                        ? string.Join(", ", volumeInfo!["categories"]!.ToObject<string[]>() ?? Array.Empty<string>())
                        : "General",

                    StockTotal = 5,
                    StockDisponible = 5
                };

                return nuevoLibro;
            }
            catch
            {
                return null;
            }
        }

        // 🟢 NUEVO MÉTODO: Búsqueda Global por palabras clave
        public async Task<List<Libro>> BuscarLibrosGlobalAsync(string query)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(query)}&maxResults=12";
            var listaLibros = new List<Libro>();

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // 💡 SOLUCIÓN CS8602: Guardamos los items en una variable
                var items = json["items"];

                // Si la variable es nula, salimos
                if (items == null) return listaLibros;

                // Al iterar sobre la variable, el compilador ya sabe 100% que no es nula
                foreach (var item in items)
                {
                    var volumeInfo = item["volumeInfo"];
                    if (volumeInfo == null) continue;

                    // Extraer el ISBN
                    string isbnFinal = "SIN-ISBN";
                    var identifiers = volumeInfo["industryIdentifiers"];
                    if (identifiers != null)
                    {
                        foreach (var id in identifiers)
                        {
                            if (id["type"]?.ToString() == "ISBN_13")
                            {
                                isbnFinal = id["identifier"]?.ToString() ?? isbnFinal;
                                break;
                            }
                        }
                    }

                    // 💡 EXTRACCIÓN DE PRECIO REAL: Google Play Store
                    var saleInfo = item["saleInfo"];
                    decimal precioReal = 15.99m; // Precio base por si no está a la venta

                    if (saleInfo != null && saleInfo["saleability"]?.ToString() == "FOR_SALE")
                    {
                        var listPrice = saleInfo["listPrice"];
                        if (listPrice != null && listPrice["amount"] != null)
                        {
                            precioReal = listPrice["amount"]!.Value<decimal>();
                        }
                    }

                    listaLibros.Add(new Libro
                    {
                        Isbn = isbnFinal,
                        Titulo = volumeInfo["title"]?.ToString() ?? "Título desconocido",
                        Autor = volumeInfo["authors"] != null
                            ? string.Join(", ", volumeInfo["authors"]!.ToObject<string[]>() ?? Array.Empty<string>())
                            : "Autor no registrado",
                        Categoria = volumeInfo["categories"] != null
                            ? string.Join(", ", volumeInfo["categories"]!.ToObject<string[]>() ?? Array.Empty<string>())
                            : "General",
                        PortadaUrl = volumeInfo["imageLinks"]?["thumbnail"]?.ToString() ?? "",
                        StockDisponible = 5,
                        Precio = precioReal // Asignamos el precio real de la API
                    });
                }
            }
            catch
            {
               
            }

            return listaLibros;
        }
    }
}