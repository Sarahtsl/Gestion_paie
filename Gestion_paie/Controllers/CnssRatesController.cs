using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;


namespace Gestion_paie.Controllers
{
   
    public class CnssRatesController : Controller
    {
        private readonly MyContext _context;

        public CnssRatesController(MyContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var rates = await _context.CnssRates.ToListAsync();
            return View(rates);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CnssRate cnssRate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cnssRate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "User"); 
            }
            return View(cnssRate);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cnssRate = await _context.CnssRates.FindAsync(id);
            if (cnssRate == null) return NotFound();

            return View(cnssRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CnssRate cnssRate)
        {
            if (id != cnssRate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cnssRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CnssRates.Any(e => e.Id == cnssRate.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cnssRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cnssRate = await _context.CnssRates.FindAsync(id);
            if (cnssRate != null)
            {
                _context.CnssRates.Remove(cnssRate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
