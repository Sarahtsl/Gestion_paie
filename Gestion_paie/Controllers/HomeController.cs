using Microsoft.AspNetCore.Mvc;

namespace Gestion_paie.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Page d'accueil
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Ceci est une page À propos.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Page de contact.";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
