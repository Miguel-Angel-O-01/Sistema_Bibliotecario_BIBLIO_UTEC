using Microsoft.AspNetCore.Mvc;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly GoogleBooksService _googleBooksService;

        // Inyectamos SOLO el servicio de Google
        public CatalogController(GoogleBooksService googleBooksService)
        {
            _googleBooksService = googleBooksService;
        }

        // 1. BARRA DE BÚSQUEDA DIRECTA A GOOGLE
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                ViewBag.Busqueda = "";
                return View(new List<Models.Libro>());
            }

            var resultadosGlobales = await _googleBooksService.BuscarLibrosGlobalAsync(q);
            ViewBag.Busqueda = q;
            return View(resultadosGlobales); // Muestra Search.cshtml
        }

        // 2. DETALLES REALES DEL LIBRO
        [HttpGet]
        public async Task<IActionResult> Detalles(string isbn)
        {
            // Esto busca el libro real con todos sus campos
            var libroReal = await _googleBooksService.BuscarPorIsbnAsync(isbn);

            // Pasamos el libro real a tu vista de 600 líneas
            return View(libroReal);
        }
    }
}