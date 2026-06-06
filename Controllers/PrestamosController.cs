using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data; // O Microsoft.EntityFrameworkCore si usas EF Core
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using System;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly BiblioUtecContext _context;

        // 1. AGREGA ESTE CONSTRUCTOR AQUÍ:
        public PrestamosController(BiblioUtecContext context)
        {
            _context = context;
        }

        // 2. TU MÉTODO SE QUEDA EXACTAMENTE IGUAL QUE COMO LO TIENES:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SolicitarPrestamo(int libroId, int usuarioId)
        {
            // 1. Buscar el libro en tu inventario local de Entity Framework
            var libro = await _context.Libros.FindAsync(libroId);

            if (libro == null)
            {
                TempData["Error"] = "El libro especificado no existe en el catálogo.";
                return RedirectToAction("Index", "Catalogo");
            }

            // 2. Validar si quedan copias físicas disponibles para prestar
            if (libro.StockDisponible <= 0)
            {
                TempData["Error"] = "Lo sentimos, todas las copias de este libro están prestadas actualmente.";
                return RedirectToAction("Detalle", "Catalogo", new { id = libroId });
            }

            // 3. Modificar el inventario local (Restar una unidad disponible)
            libro.StockDisponible--;

            // 4. Crear el registro del préstamo en la base de datos
            var nuevoPrestamo = new Prestamo
            {
                LibroId = libroId,
                UsuarioId = usuarioId,
                FechaSalida = DateTime.Now,
                FechaMaxDevolucion = DateTime.Now.AddDays(14), // 2 semanas de préstamo
                Estado = "Activo"
            };

            _context.Prestamos.Add(nuevoPrestamo);

            // 5. Guardar de forma transaccional en la base de datos local
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"El préstamo de '{libro.Titulo}' se registró correctamente.";
            return RedirectToAction("MisPrestamos", "Usuario");
        }

        // 🟢 NUEVA ACCIÓN: Devuelve la vista de "Mis Transacciones"
        [HttpGet]
        public async Task<IActionResult> MisPrestamos()
        {
            // Más adelante, aquí obtendremos el ID del usuario de la sesión para filtrar sus préstamos exactos.
            // Ejemplo: int usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            // Usamos .Include() para traer los datos del Libro relacionado al préstamo
            var prestamosDelUsuario = await _context.Prestamos
                .Include(p => p.Libro)
                .ToListAsync();

            // Retornamos la vista (por ahora mostrará el diseño estático, luego le pasaremos la lista real)
            return View();
        }
    }
}
