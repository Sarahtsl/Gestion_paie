using Microsoft.AspNetCore.Mvc;
using Gestion_paie.Models;
using Gestion_paie.DataBase;

public class AccountController : Controller
{
    private readonly MyContext _context;

    public AccountController(MyContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null || user.PasswordHash != password)
        {
            ViewBag.Error = "Email ou mot de passe incorrect";
            return View();
        }

        var role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;

        HttpContext.Session.SetString("UserRole", role ?? "");
        HttpContext.Session.SetString("UserName", user.Username ?? "");

        if (role == "Administrateur")
        {
            return RedirectToAction("Index", "User");
        }
        else if (role == "Employe")
        {
            return RedirectToAction("EmployeDashboard");
        }
        else if (role == "Resource Humain")
        {
            return RedirectToAction("Index", "RH");
        }

        return View();
    }


    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }

    public IActionResult EmployeDashboard()
    {
        return Content("👋 Bonjour Employé !");
    }

    
}
