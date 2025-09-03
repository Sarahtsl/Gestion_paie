using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Gestion_paie.Controllers
{
    public class TaxBracketsController : Controller
    {
        private readonly MyContext _context;

        public TaxBracketsController(MyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var taxBrackets = await _context.TaxBrackets.ToListAsync();
            return View(taxBrackets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaxBracket taxBracket)
        {
            if (ModelState.IsValid)
            {
                _context.TaxBrackets.Add(taxBracket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taxBracket);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var taxBracket = await _context.TaxBrackets.FindAsync(id);
            if (taxBracket == null)
            {
                return NotFound();
            }
            return View(taxBracket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaxBracket taxBracket)
        {
            if (id != taxBracket.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taxBracket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxBracketExists(taxBracket.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taxBracket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var taxBracket = await _context.TaxBrackets.FindAsync(id);
            if (taxBracket != null)
            {
                _context.TaxBrackets.Remove(taxBracket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaxBracketExists(int id)
        {
            return _context.TaxBrackets.Any(e => e.Id == id);
        }
    }
}
