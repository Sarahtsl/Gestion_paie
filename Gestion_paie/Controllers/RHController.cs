using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_paie.Controllers
{
    public class RHController : Controller
    {
        private readonly MyContext _context;

        public RHController(MyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.User)
                .ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDowns();

            var employee = new Employee
            {
                Status = "ACTIVE",
                DependentsCount = 0,
                BaseSalary = 0m,
                BankAccount = "",
                BankCode = "",
                MaritalStatus = ""
            };

            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            employee.Status ??= "ACTIVE";
            employee.DependentsCount = employee.DependentsCount;
            employee.BaseSalary = employee.BaseSalary == 0 ? 0 : employee.BaseSalary;
            employee.BankAccount ??= "";
            employee.BankCode ??= "";
            employee.MaritalStatus ??= "";

            if (employee.CompanyId == null)
            {
                    employee.CompanyId = 1;
            }

            if (_context.Employees.Any(e => e.CIN == employee.CIN))
            {
                ModelState.AddModelError("CIN", "Ce CIN est déjà utilisé.");
            }

            if (_context.Employees.Any(e => e.CNSSNumber == employee.CNSSNumber))
            {
                ModelState.AddModelError("CNSSNumber", "Ce numéro CNSS est déjà utilisé.");
            }

            if (_context.Employees.Any(e => e.BankAccount == employee.BankAccount))
            {
                ModelState.AddModelError("BankAccount", "Ce compte bancaire est déjà utilisé.");
            }


            if (string.IsNullOrEmpty(employee.EmployeeNumber))
            {
                var lastEmployeeNumber = _context.Employees
                    .OrderByDescending(e => e.EmployeeNumber)
                    .Select(e => e.EmployeeNumber)
                    .FirstOrDefault();

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(lastEmployeeNumber))
                    nextNumber = int.Parse(lastEmployeeNumber.Substring(3)) + 1;

                employee.EmployeeNumber = $"EMP{nextNumber:D4}";
            }

            if (ModelState.IsValid)
            {
                employee.CreatedAt = DateTime.Now;
                employee.UpdatedAt = DateTime.Now;

                _context.Employees.Add(employee);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns();
            return View(employee);
        }

        private void PopulateDropDowns()
        {
            ViewBag.Users = _context.Users
                .OrderBy(u => u.Username)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Username
                })
                .ToList();

            ViewBag.Companies = _context.Companies
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            bool cinExists = await _context.Employees
                .AnyAsync(e => e.CIN == employee.CIN && e.Id != employee.Id);
            if (cinExists)
            {
                ModelState.AddModelError("CIN", "Le CIN est déjà utilisé par un autre employé.");
            }

            if (!string.IsNullOrEmpty(employee.CNSSNumber))
            {
                bool cnssExists = await _context.Employees
                    .AnyAsync(e => e.CNSSNumber == employee.CNSSNumber && e.Id != employee.Id);
                if (cnssExists)
                {
                    ModelState.AddModelError("CNSSNumber", "Le numéro CNSS est déjà utilisé par un autre employé.");
                }
            }

            if (!string.IsNullOrEmpty(employee.BankAccount))
            {
                bool bankExists = await _context.Employees
                    .AnyAsync(e => e.BankAccount == employee.BankAccount && e.Id != employee.Id);
                if (bankExists)
                {
                    ModelState.AddModelError("BankAccount", "Le compte bancaire est déjà utilisé par un autre employé.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(employee);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

            }
          
            return RedirectToAction(nameof(Index));
        }
        public IActionResult VerifyCIN(string cin, int id)
        {
            bool exists = _context.Employees.Any(e => e.CIN == cin && e.Id != id);
            if (exists)
                return Json($"Le CIN '{cin}' est déjà utilisé.");
            return Json(true);
        }

        public IActionResult VerifyCNSS(string cnssNumber, int id)
        {
            bool exists = _context.Employees.Any(e => e.CNSSNumber == cnssNumber && e.Id != id);
            if (exists)
                return Json($"Le numéro CNSS '{cnssNumber}' est déjà utilisé.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyBankAccount(string bankAccount, int id)
        {
            bool exists = _context.Employees.Any(e => e.BankAccount == bankAccount && e.Id != id);
            if (exists)
            {
                return Json($"Le compte bancaire '{bankAccount}' est déjà utilisé.");
            }
            return Json(true);
        }


    }
}
