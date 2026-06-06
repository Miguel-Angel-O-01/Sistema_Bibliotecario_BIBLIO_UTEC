using Microsoft.AspNetCore.Mvc;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Controllers
{
    public class CheckoutController : Controller
    {
        [HttpGet]
        public IActionResult Pago()
        {
            return View();
        }
    }
}