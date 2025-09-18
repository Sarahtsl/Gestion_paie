using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Authorization;
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

        // INDEX
        public async Task<IActionResult> Index()
        {
            var periods = await _context.PayrollPeriods
                .Include(p => p.Company)
                .OrderByDescending(p => p.PeriodYear)
                .ThenByDescending(p => p.PeriodMonth)
                .ToListAsync();

            return View(periods);
        }

        // CREATE (GET) — verrouillé sur la 1ʳᵉ société
        public IActionResult Create()
        {
            var model = new PayrollPeriod
            {
                Status = PayrollPeriodStatus.DRAFT,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            var companies = _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            if (companies.Count == 0)
            {
                // Pas de sociétés : on affiche la vue avec un message clair
                ViewBag.LockCompanySelect = true;
                ModelState.AddModelError(nameof(model.CompanyId), "Aucune société n'est configurée.");
                PopulateDropDowns(null);
                return View(model);
            }

            // Imposer la 1ʳᵉ société et verrouiller
            model.CompanyId = companies[0].Id;
            ViewBag.LockCompanySelect = true;

            PopulateDropDowns(model.CompanyId);
            return View(model);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollPeriod model)
        {
            // Réimposer côté serveur (sécurité)
            var firstCompanyId = await _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (firstCompanyId == 0)
            {
                ModelState.AddModelError(nameof(model.CompanyId), "Aucune société n'est configurée.");
            }
            else
            {
                model.CompanyId = firstCompanyId;
            }

            // Validations
            if (model.PeriodMonth is < 1 or > 12)
                ModelState.AddModelError(nameof(model.PeriodMonth), "Le mois doit être entre 1 et 12.");

            if (model.EndDate < model.StartDate)
                ModelState.AddModelError(nameof(model.EndDate), "La date de fin doit être ≥ la date de début.");

            bool alreadyExists = await _context.PayrollPeriods.AnyAsync(p =>
                p.CompanyId == model.CompanyId &&
                p.PeriodYear == model.PeriodYear &&
                p.PeriodMonth == model.PeriodMonth);

            if (alreadyExists)
                ModelState.AddModelError(string.Empty, "Une période pour cette société et ce mois existe déjà.");

            if (!ModelState.IsValid)
            {
                ViewBag.LockCompanySelect = true; // garder verrouillé au retour
                PopulateDropDowns(model.CompanyId);
                return View(model);
            }

            _context.PayrollPeriods.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET) — par défaut non verrouillé
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var period = await _context.PayrollPeriods.FindAsync(id.Value);
            if (period == null) return NotFound();

            ViewBag.LockCompanySelect = false; // mettre true si tu veux le verrouiller ici
            PopulateDropDowns(period.CompanyId);
            return View(period);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollPeriod model)
        {
            if (id != model.Id) return NotFound();

            if (model.CompanyId is null)
                ModelState.AddModelError(nameof(model.CompanyId), "Veuillez sélectionner une société.");

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
                ViewBag.LockCompanySelect = false;
                PopulateDropDowns(model.CompanyId);
                return View(model);
            }

            // Ne pas écraser CreatedAt
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

        // DELETE
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

        // Helper: liste sociétés
        private void PopulateDropDowns(int? selectedCompanyId = null)
        {
            var companies = _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.Companies = new SelectList(
                companies,
                nameof(Company.Id),
                nameof(Company.Name),
                selectedCompanyId
            );
        }
    }
}