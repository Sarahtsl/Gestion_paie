using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_paie.Controllers
{
    public class PayrollPeriodsController : Controller
    {
        private readonly MyContext _context;

        public PayrollPeriodsController(MyContext context)
        {
            _context = context;
        }

        // GET: PayrollPeriods
        public async Task<IActionResult> Index()
        {
            var periods = await _context.PayrollPeriods
                .Include(p => p.Company)
                .OrderByDescending(p => p.PeriodYear)
                .ThenByDescending(p => p.PeriodMonth)
                .ToListAsync();
            return View(periods);
        }

        // GET: PayrollPeriods/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View(new PayrollPeriod
            {
                Status = PayrollPeriodStatus.DRAFT,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            });
        }

        // POST: PayrollPeriods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollPeriod model)
        {
            // validations
            if (model.PeriodMonth is < 1 or > 12)
                ModelState.AddModelError(nameof(model.PeriodMonth), "Le mois doit être entre 1 et 12.");
            if (model.EndDate < model.StartDate)
                ModelState.AddModelError(nameof(model.EndDate), "La date de fin doit être ≥ la date de début.");

            // unicité société+année+mois
            bool alreadyExists = await _context.PayrollPeriods.AnyAsync(p =>
                p.CompanyId == model.CompanyId &&
                p.PeriodYear == model.PeriodYear &&
                p.PeriodMonth == model.PeriodMonth);

            if (alreadyExists)
                ModelState.AddModelError(string.Empty, "Une période pour cette société et ce mois existe déjà.");

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(model.CompanyId);
                return View(model);
            }

            _context.PayrollPeriods.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: PayrollPeriods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var period = await _context.PayrollPeriods.FindAsync(id.Value);
            if (period == null) return NotFound();

            PopulateDropDowns(period.CompanyId);
            return View(period);
        }

        // POST: PayrollPeriods/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollPeriod model)
        {
            if (id != model.Id) return NotFound();

            if (model.PeriodMonth is < 1 or > 12)
                ModelState.AddModelError(nameof(model.PeriodMonth), "Le mois doit être entre 1 et 12.");
            if (model.EndDate < model.StartDate)
                ModelState.AddModelError(nameof(model.EndDate), "La date de fin doit être ≥ la date de début.");

            bool duplicate = await _context.PayrollPeriods.AnyAsync(p =>
                p.Id != model.Id &&
                p.CompanyId == model.CompanyId &&
                p.PeriodYear == model.PeriodYear &&
                p.PeriodMonth == model.PeriodMonth);

            if (duplicate)
                ModelState.AddModelError(string.Empty, "Une période pour cette société et ce mois existe déjà.");

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(model.CompanyId);
                return View(model);
            }

            // ne pas modifier CreatedAt (valeur générée DB)
            _context.Entry(model).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.PayrollPeriods.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // --- SUPPRESSION DIRECTE (pas de vue Delete) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _context.PayrollPeriods.FindAsync(id);
            if (period != null)
            {
                _context.PayrollPeriods.Remove(period);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // ------------------------------------------------

        // helper
        private void PopulateDropDowns(int? selectedCompanyId = null)
        {
            ViewBag.Companies = _context.Companies
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = selectedCompanyId != null && c.Id == selectedCompanyId
                })
                .ToList();
        }
    }
}
