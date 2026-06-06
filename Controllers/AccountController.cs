using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Necesario para la Sesión
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class AccountController : Controller
    {
        private readonly BiblioUtecContext _context;
        private readonly GoogleAuthService _googleAuthService; // Inyectamos tu servicio de Google

        // Constructor que recibe la Base de Datos y el Servicio de Google
        public AccountController(BiblioUtecContext context, GoogleAuthService googleAuthService)
        {
            _context = context;
            _googleAuthService = googleAuthService;
        }

        // ==========================================
        // 🟢 RUTAS GET (Para abrir las pantallas)
        // ==========================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Billetera()
        {
            return View();
        }

        // ==========================================
        // 🔵 RUTAS POST (Para procesar formularios)
        // ==========================================

        // 1. Registro Manual
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string carnet, string email, string password)
        {
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Prestamos = new List<Prestamo>(),
                Carnet = carnet ?? "Pendiente",
                Password = password
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        // 2. Inicio de Sesión con Google (El nuevo método que agregamos)
        [HttpPost]
        public async Task<IActionResult> GoogleLogin(string idToken)
        {
            // Validamos que el token es auténtico de Google
            var payload = await _googleAuthService.ValidarTokenGoogleAsync(idToken);

            if (payload == null)
            {
                TempData["Error"] = "La autenticación con Google falló o el token expiró.";
                return RedirectToAction("Login");
            }

            // Buscamos si el usuario ya existe en tu SQLite
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (usuario == null)
            {
                // Si es primera vez, lo registramos automáticamente
                usuario = new Usuario
                {
                    Nombre = payload.Name,
                    Email = payload.Email,
                    Carnet = "Google-SSO",
                    Password = "GoogleAuthUser"
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            // Iniciamos sesión en la aplicación
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);

            TempData["Exito"] = $"¡Bienvenido {usuario.Nombre}!";
            return RedirectToAction("Index", "Home");
        }
    }
}