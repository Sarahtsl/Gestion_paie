using Gestion_paie.DataBase; 
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Gestion_paie.Controllers
{
    public class TaxDeductionController : Controller
    {
        private readonly MyContext _context;

        public TaxDeductionController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var deductions = _context.TaxDeductions.ToList();
            return View(deductions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaxDeduction deduction)
        {
            if (ModelState.IsValid)
            {
                _context.TaxDeductions.Add(deduction);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(deduction);
        }
       
        public IActionResult Edit(int id)
        {
            var deduction = _context.TaxDeductions.Find(id);
            if (deduction == null)
            {
                return NotFound();
            }
            return View(deduction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaxDeduction deduction)
        {
            if (id != deduction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(deduction);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(deduction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var deduction = _context.TaxDeductions.Find(id);
            if (deduction != null)
            {
                _context.TaxDeductions.Remove(deduction);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
