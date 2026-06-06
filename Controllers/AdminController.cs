using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using Sistema_Bibliotecario_BIBLIO_UTEC.Services;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class AdminController : Controller
    {
        private readonly BiblioUtecContext _context;
        private readonly GoogleBooksService _booksService;
        private readonly AlgoliaSearchService _algoliaService;

        // Inyección automática de dependencias desde Program.cs
        public AdminController(BiblioUtecContext context, GoogleBooksService booksService, AlgoliaSearchService algoliaService)
        {
            _context = context;
            _booksService = booksService;
            _algoliaService = algoliaService;
        }

        // ==========================================
        // 🟢 NUEVA ACCIÓN: Vista del Dashboard
        // ==========================================
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // Vista principal del inventario del Administrador
        public async Task<IActionResult> Inventario()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // Acción que procesa el formulario de ISBN para el autollenado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarLibroPorIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                TempData["Error"] = "El código ISBN es obligatorio.";
                return RedirectToAction(nameof(Inventario));
            }

            // 1. Validar que el libro no esté duplicado en SQLite
            var existeEnBaseLocal = await _context.Libros.AnyAsync(l => l.Isbn == isbn);
            if (existeEnBaseLocal)
            {
                TempData["Error"] = "Este ejemplar ya se encuentra registrado en el sistema local.";
                return RedirectToAction(nameof(Inventario));
            }

            // 2. Consumir el servicio externo de Google Books API
            Libro? nuevoLibro = await _booksService.BuscarPorIsbnAsync(isbn);
            if (nuevoLibro == null)
            {
                TempData["Error"] = "No se localizaron metadatos para este ISBN en los servidores de Google.";
                return RedirectToAction(nameof(Inventario));
            }

            // 3. Guardar el libro en tu base de datos SQLite local
            _context.Libros.Add(nuevoLibro);
            await _context.SaveChangesAsync(); // Entity Framework genera el nuevo Id aquí automáticamente

            // 4. Indexar asíncronamente en la nube comercial de Algolia
            await _algoliaService.IndexarLibroAsync(
                nuevoLibro.Id,
                nuevoLibro.Titulo,
                nuevoLibro.Autor,
                nuevoLibro.Isbn,
                nuevoLibro.Categoria,
                nuevoLibro.PortadaUrl??""
            );

            TempData["Exito"] = $"¡Libro '{nuevoLibro.Titulo}' importado de Google Books e indexado en Algolia con éxito!";
            return RedirectToAction(nameof(Inventario));
        }
    }
}