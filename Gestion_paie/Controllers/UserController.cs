using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Gestion_paie.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_paie.Controllers
{
   
    public class UserController : Controller
    {
        private readonly MyContext _context;

        public UserController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                foreach (var err in errors)
                    System.Diagnostics.Debug.WriteLine(err);

                ViewBag.Roles = _context.Roles.ToList();
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Index()
        {
            var users = _context.Users
                                .Select(u => new {
                                    u.Id,
                                    u.Username,
                                    u.Email,
                                    RoleName = u.Role.Name
                                }).ToList();
            return View(users);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find(model.Id);
                if (existingUser == null) return NotFound();

                existingUser.Username = model.Username;
                existingUser.Email = model.Email;
                existingUser.RoleId = model.RoleId;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }

}
