using Microsoft.AspNetCore.Mvc;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using System.Threading.Tasks;
using System.Linq;
using System; // Agregado para usar DateTime.Now

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    // CORRECCIÓN: Dos 'r', escrito correctamente para que la ruta sea /Carrito/Index
    public class CarritoController : Controller
    {
        private readonly BiblioUtecContext _context;

        public CarritoController(BiblioUtecContext context)
        {
            _context = context;
        }

        // 1. Ver los libros que el alumno tiene agregados en su carrito temporal
        [HttpGet]
        public IActionResult Index()
        {
            // Ojo: Esto requiere que tengas tu clase de extensión SessionExtensions configurada
            var carrito = HttpContext.Session.Get<Carrito>("Carrito") ?? new Carrito();
            return View(carrito);
        }

        // 2. Acción para añadir un libro desde el catálogo hacia el carrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int libroId)
        {
            var libro = await _context.Libros.FindAsync(libroId);
            if (libro == null || libro.StockDisponible <= 0)
            {
                TempData["Error"] = "El libro no está disponible para reserva.";
                return RedirectToAction("Index", "Catalog");
            }

            var carrito = HttpContext.Session.Get<Carrito>("Carrito") ?? new Carrito();
            carrito.AgregarItem(libro);

            HttpContext.Session.Set("Carrito", carrito);

            TempData["Exito"] = $"'{libro.Titulo}' se añadió a tus solicitudes de reserva.";
            return RedirectToAction("Index", "Catalog");
        }

        // 3. Acción para remover un libro si el alumno se arrepiente
        [HttpPost]
        public IActionResult RemoverDelCarrito(int libroId)
        {
            var carrito = HttpContext.Session.Get<Carrito>("Carrito") ?? new Carrito();
            carrito.RemoverItem(libroId);

            HttpContext.Session.Set("Carrito", carrito);
            return RedirectToAction(nameof(Index));
        }

        // 4. Confirmar la reserva y validar ReCaptcha
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(string captchaToken, int usuarioIdActual)
        {
            var captchaService = HttpContext.RequestServices.GetService(typeof(CaptchaService)) as CaptchaService;

            if (captchaService != null)
            {
                bool esHumano = await captchaService.ValidarTokenAsync(captchaToken);
                if (!esHumano)
                {
                    TempData["Error"] = "La verificación de reCAPTCHA falló. Por favor intenta de nuevo.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var carrito = HttpContext.Session.Get<Carrito>("Carrito") ?? new Carrito();
            if (!carrito.Items.Any())
            {
                TempData["Error"] = "Tu solicitud de reserva está vacía.";
                return RedirectToAction("Index", "Catalog");
            }

            foreach (var item in carrito.Items)
            {
                var libroDb = await _context.Libros.FindAsync(item.LibroId);
                if (libroDb != null && libroDb.StockDisponible > 0)
                {
                    libroDb.StockDisponible--;

                    var nuevoPrestamo = new Prestamo
                    {
                        LibroId = libroDb.Id,
                        UsuarioId = usuarioIdActual,
                        FechaSalida = DateTime.Now,
                        FechaMaxDevolucion = DateTime.Now.AddDays(14),
                        Estado = "Activo"
                    };

                    _context.Prestamos.Add(nuevoPrestamo);
                }
            }

            await _context.SaveChangesAsync();
            carrito.Limpiar();
            HttpContext.Session.Set("Carrito", carrito);

            TempData["Exito"] = "¡Tus libros han sido reservados con éxito! Puedes pasar a recogerlos.";
            return RedirectToAction("MisPrestamos", "Prestamos");
        }
    }
}