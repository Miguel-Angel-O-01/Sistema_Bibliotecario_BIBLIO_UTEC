using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using Sistema_Bibliotecario_BIBLIO_UTEC.Services;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class HomeController : Controller
    {
        private readonly BiblioUtecContext _context;
        private readonly NytBooksService _nytService;

        public HomeController(BiblioUtecContext context, NytBooksService nytService)
        {
            _context = context;
            _nytService = nytService;
        }

        public async Task<IActionResult> Index()
        {
            var librosLocales = await _context.Libros.Take(6).ToListAsync();

            // ⚡ SIEMBRA AUTOMÁTICA INTELIGENTE (Ocurre solo si tu .db está vacío)
            if (librosLocales == null || !librosLocales.Any())
            {
                var googleService = HttpContext.RequestServices.GetService(typeof(GoogleBooksService)) as GoogleBooksService;
                var algoliaService = HttpContext.RequestServices.GetService(typeof(AlgoliaSearchService)) as AlgoliaSearchService;

                if (googleService != null && algoliaService != null)
                {
                    // Un set de ISBNs famosos comerciales (Harry Potter, El Quijote, etc)
                    var listadoIsbns = new List<string> { "9788498387087", "9788445000663", "9788497592208", "9788437604947", "9788420658872", "9788467034028" };

                    foreach (var isbn in listadoIsbns)
                    {
                        var libroDescargado = await googleService.BuscarPorIsbnAsync(isbn);
                        if (libroDescargado != null)
                        {
                            _context.Libros.Add(libroDescargado);
                            await _context.SaveChangesAsync(); // Inyecta en tu SQLite

                            // Indexa en Algolia para tu buscador predictivo superior
                            await algoliaService.IndexarLibroAsync
                                (libroDescargado.Id, libroDescargado.Titulo, 
                                libroDescargado.Autor, 
                                libroDescargado.Isbn, 
                                libroDescargado.Categoria,
                                libroDescargado.PortadaUrl ?? ""

                                );
                        }
                    }
                    // Recargamos la lista local con los libros recién sembrados
                    librosLocales = await _context.Libros.Take(6).ToListAsync();
                }
            }

            // Descarga los recomendados del New York Times de internet
            var bestsellers = await _nytService.ObtenerBestsellersAsync();
            ViewBag.Bestsellers = bestsellers;

            return View(librosLocales);
        }



    }
}
