using Microsoft.AspNetCore.Mvc;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly BiblioUtecContext _context;

        // Inyectamos la base de datos en el constructor
        public RegisterController(BiblioUtecContext context)
        {
            _context = context;
        }

        // 1. GET: Abre la pantalla visual de registro
        // Ruta: /Register/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Retorna la vista. Si tu archivo está en Views/Account/Register.cshtml, 
            // usamos la ruta exacta para no tener que mover el archivo de carpeta:
            return View("~/Views/Account/Register.cshtml");
        }

        // 2. POST: Captura los datos del formulario y los guarda en SQLite
        // Ruta: /Register/Index
        [HttpPost]
        public async Task<IActionResult> Index(string nombre, string carnet, string email, string password)
        {
            // Verificamos si el correo ya existe para no duplicar cuentas
            // (Requiere using Microsoft.EntityFrameworkCore;)
            // var existe = await _context.Usuarios.AnyAsync(u => u.Email == email);
            // if(existe) { /* Mostrar error */ }


            // Creamos el nuevo objeto Usuario basado en tu modelo
            // Creamos el nuevo objeto Usuario basado en tu modelo
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Prestamos = new List<Prestamo>(),

              
                Carnet = carnet ?? "Pendiente",
                Password = password
            };

            // Lo agregamos a la tabla de Usuarios y guardamos los cambios
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Una vez guardado exitosamente en la base de datos, lo enviamos al Login
            return RedirectToAction("Login", "Account");
        }
    }
}